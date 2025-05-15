using MediatR;
using TheSSS.DICOMViewer.Application.DicomImport.DTOs;
using TheSSS.DICOMViewer.Domain.Interfaces;
using TheSSS.DICOMViewer.Infrastructure.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TheSSS.DICOMViewer.Application.DicomImport.Commands.Handlers
{
    public class ImportDicomFilesCommandHandler : IRequestHandler<ImportDicomFilesCommand, ImportResultDto>
    {
        private readonly IDicomComplianceValidatorAdapter _complianceValidator;
        private readonly IDicomFileStoreAdapter _fileStore;
        private readonly IMetadataRepositoryAdapter _metadataRepository;
        private readonly IAuditLogRepositoryAdapter _auditLog;

        public ImportDicomFilesCommandHandler(
            IDicomComplianceValidatorAdapter complianceValidator,
            IDicomFileStoreAdapter fileStore,
            IMetadataRepositoryAdapter metadataRepository,
            IAuditLogRepositoryAdapter auditLog)
        {
            _complianceValidator = complianceValidator;
            _fileStore = fileStore;
            _metadataRepository = metadataRepository;
            _auditLog = auditLog;
        }

        public async Task<ImportResultDto> Handle(ImportDicomFilesCommand request, CancellationToken cancellationToken)
        {
            var result = new ImportResultDto();
            var filesToProcess = GetFilesFromRequest(request);

            await _auditLog.LogEventAsync("ImportStarted", "DICOM import initiated", "InProgress");

            foreach (var filePath in filesToProcess)
            {
                try
                {
                    using var stream = await _fileStore.GetDicomFileAsync(filePath);
                    var validationResult = await _complianceValidator.ValidateComplianceAsync(stream);

                    if (!validationResult.IsCompliant)
                    {
                        await HandleNonCompliantFile(filePath, validationResult, result);
                        continue;
                    }

                    var storagePath = await _fileStore.StoreDicomFileAsync(filePath, 
                        validationResult.PatientId, 
                        validationResult.StudyInstanceUid,
                        validationResult.SeriesInstanceUid,
                        validationResult.SopInstanceUid);

                    await PersistMetadata(validationResult, storagePath);
                    result.SuccessfullyImportedCount++;
                    result.ProcessedFileNames.Add(filePath);
                }
                catch (System.Exception ex)
                {
                    result.FailedOrQuarantinedCount++;
                    result.ErrorMessagesOrQuarantineReasons.Add($"Error processing {filePath}: {ex.Message}");
                }
            }

            await _auditLog.LogEventAsync("ImportCompleted", 
                $"Processed {filesToProcess.Count} files. Success: {result.SuccessfullyImportedCount}, Failed: {result.FailedOrQuarantinedCount}", 
                "Completed");

            return result;
        }

        private List<string> GetFilesFromRequest(ImportDicomFilesCommand request)
        {
            if (!string.IsNullOrEmpty(request.DirectoryPath))
                return System.IO.Directory.GetFiles(request.DirectoryPath, "*", System.IO.SearchOption.AllDirectories).ToList();
            
            return request.FilePaths.ToList();
        }

        private async Task HandleNonCompliantFile(string filePath, DicomValidationResultDto validationResult, ImportResultDto result)
        {
            result.FailedOrQuarantinedCount++;
            var errorMessage = $"Non-compliant DICOM file: {string.Join(", ", validationResult.ValidationErrors)}";
            result.ErrorMessagesOrQuarantineReasons.Add(errorMessage);
            
            await _auditLog.LogEventAsync("FileQuarantined", 
                $"File {filePath} quarantined. Reasons: {errorMessage}", 
                "Failed");
            
            await _fileStore.MoveToQuarantineAsync(filePath, errorMessage);
        }

        private async Task PersistMetadata(DicomValidationResultDto validationResult, string storagePath)
        {
            await _metadataRepository.AddPatientAsync(new PatientDto(
                validationResult.PatientId,
                validationResult.PatientName,
                validationResult.PatientBirthDate,
                validationResult.PatientSex));

            await _metadataRepository.AddStudyAsync(new StudyDto(
                validationResult.StudyInstanceUid,
                validationResult.PatientId,
                validationResult.StudyDate,
                validationResult.StudyTime,
                validationResult.AccessionNumber,
                validationResult.StudyDescription));

            await _metadataRepository.AddSeriesAsync(new SeriesDto(
                validationResult.SeriesInstanceUid,
                validationResult.StudyInstanceUid,
                validationResult.Modality,
                validationResult.SeriesNumber,
                validationResult.SeriesDescription));

            await _metadataRepository.AddInstanceAsync(new InstanceDto(
                validationResult.SopInstanceUid,
                validationResult.SeriesInstanceUid,
                validationResult.SopClassUid,
                validationResult.InstanceNumber,
                storagePath,
                validationResult.PhotometricInterpretation,
                validationResult.Rows,
                validationResult.Columns));
        }
    }
}