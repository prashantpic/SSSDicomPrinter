using MediatR;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TheSSS.DICOMViewer.Application.Common.DTOs;
using TheSSS.DICOMViewer.Application.Common.Exceptions;
using TheSSS.DICOMViewer.Application.DicomImport.Commands;
using TheSSS.DICOMViewer.Application.Interfaces.Domain;
using TheSSS.DICOMViewer.Application.Interfaces.Infrastructure;
using Dicom;

namespace TheSSS.DICOMViewer.Application.DicomImport.Commands.Handlers;

public class ImportDicomFilesCommandHandler : IRequestHandler<ImportDicomFilesCommand, ImportResultDto>
{
    private readonly IDicomComplianceValidatorAdapter _complianceValidator;
    private readonly IDicomFileStoreAdapter _fileStore;
    private readonly IMetadataRepositoryAdapter _metadataRepository;
    private readonly IAuditLogRepositoryAdapter _auditLogger;

    public ImportDicomFilesCommandHandler(
        IDicomComplianceValidatorAdapter complianceValidator,
        IDicomFileStoreAdapter fileStore,
        IMetadataRepositoryAdapter metadataRepository,
        IAuditLogRepositoryAdapter auditLogger)
    {
        _complianceValidator = complianceValidator;
        _fileStore = fileStore;
        _metadataRepository = metadataRepository;
        _auditLogger = auditLogger;
    }

    public async Task<ImportResultDto> Handle(ImportDicomFilesCommand request, CancellationToken cancellationToken)
    {
        var result = new ImportResultDto();
        var files = ResolveFiles(request);

        await _auditLogger.LogEventAsync("DICOM_IMPORT_START", $"Processing {files.Count} files", "STARTED");

        foreach (var filePath in files)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    HandleMissingFile(filePath, result);
                    continue;
                }

                using var stream = File.OpenRead(filePath);
                var validationResult = await _complianceValidator.ValidateComplianceAsync(stream);
                
                if (!validationResult.IsCompliant)
                {
                    await HandleNonCompliantFile(filePath, validationResult, result);
                    continue;
                }

                stream.Seek(0, SeekOrigin.Begin);
                var dicomFile = await ProcessCompliantFile(filePath, stream, result);
                await PersistMetadata(dicomFile, filePath);
            }
            catch (Exception ex)
            {
                HandleProcessingError(filePath, ex, result);
            }
        }

        await _auditLogger.LogEventAsync("DICOM_IMPORT_COMPLETE", 
            $"Success: {result.SuccessfullyImportedCount}, Failed: {result.FailedOrQuarantinedCount}", 
            "COMPLETED");

        return result;
    }

    private List<string> ResolveFiles(ImportDicomFilesCommand request)
    {
        if (request.IsDirectoryImport && !string.IsNullOrEmpty(request.DirectoryPath))
        {
            return Directory.Exists(request.DirectoryPath) 
                ? Directory.EnumerateFiles(request.DirectoryPath, "*", SearchOption.AllDirectories).ToList()
                : new List<string>();
        }
        return request.FilePaths?.ToList() ?? new List<string>();
    }

    private async Task<DicomFile> ProcessCompliantFile(string filePath, Stream stream, ImportResultDto result)
    {
        var dicomFile = await DicomFile.OpenAsync(stream);
        var storedPath = await _fileStore.StoreDicomFileAsync(
            filePath,
            dicomFile.Dataset.GetString(DicomTag.PatientID),
            dicomFile.Dataset.GetString(DicomTag.StudyInstanceUID),
            dicomFile.Dataset.GetString(DicomTag.SeriesInstanceUID),
            dicomFile.Dataset.GetString(DicomTag.SOPInstanceUID));

        result.SuccessfullyImportedCount++;
        result.ProcessedFileNames.Add(Path.GetFileName(filePath));
        
        await _auditLogger.LogEventAsync("DICOM_IMPORT_SUCCESS", 
            $"Stored: {storedPath}", "SUCCESS");

        return dicomFile;
    }

    private async Task PersistMetadata(DicomFile dicomFile, string originalPath)
    {
        var dataset = dicomFile.Dataset;
        var patientDto = new PatientDto(
            dataset.GetString(DicomTag.PatientID),
            dataset.GetString(DicomTag.PatientName),
            dataset.GetValue<DateTime?>(DicomTag.PatientBirthDate, 0, null),
            dataset.GetString(DicomTag.PatientSex));

        var studyDto = new StudyDto(
            dataset.GetString(DicomTag.StudyInstanceUID),
            patientDto.PatientId,
            dataset.GetString(DicomTag.StudyID),
            dataset.GetValue<DateTime?>(DicomTag.StudyDate, 0, null),
            dataset.GetString(DicomTag.StudyDescription));

        var seriesDto = new SeriesDto(
            dataset.GetString(DicomTag.SeriesInstanceUID),
            studyDto.StudyInstanceUid,
            dataset.GetString(DicomTag.SeriesNumber),
            dataset.GetString(DicomTag.Modality),
            dataset.GetString(DicomTag.SeriesDescription));

        var instanceDto = new InstanceDto(
            dataset.GetString(DicomTag.SOPInstanceUID),
            seriesDto.SeriesInstanceUid,
            dataset.GetString(DicomTag.InstanceNumber),
            dataset.GetString(DicomTag.SOPClassUID),
            originalPath,
            false,
            null);

        await _metadataRepository.AddPatientAsync(patientDto);
        await _metadataRepository.AddStudyAsync(studyDto);
        await _metadataRepository.AddSeriesAsync(seriesDto);
        await _metadataRepository.AddInstanceAsync(instanceDto);
    }

    private async Task HandleNonCompliantFile(string filePath, DicomValidationResultDto validationResult, ImportResultDto result)
    {
        result.FailedOrQuarantinedCount++;
        result.ErrorMessagesOrQuarantineReasons.Add(
            $"Non-compliant: {Path.GetFileName(filePath)} - {string.Join(", ", validationResult.ValidationErrors)}");
        
        await _fileStore.MoveToQuarantineAsync(filePath, "Validation failed");
        await _auditLogger.LogEventAsync("DICOM_IMPORT_QUARANTINE", 
            $"File quarantined: {filePath}", "QUARANTINED");
    }

    private void HandleMissingFile(string filePath, ImportResultDto result)
    {
        result.FailedOrQuarantinedCount++;
        result.ErrorMessagesOrQuarantineReasons.Add($"File not found: {filePath}");
    }

    private void HandleProcessingError(string filePath, Exception ex, ImportResultDto result)
    {
        result.FailedOrQuarantinedCount++;
        result.ErrorMessagesOrQuarantineReasons.Add(
            $"Error processing {Path.GetFileName(filePath)}: {ex.Message}");
    }
}