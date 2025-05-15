using MediatR;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using TheSSS.DICOMViewer.Application.Anonymization.Commands;
using TheSSS.DICOMViewer.Application.Anonymization.DTOs;
using TheSSS.DICOMViewer.Application.Common.Exceptions;
using TheSSS.DICOMViewer.Application.Interfaces.Domain;
using TheSSS.DICOMViewer.Application.Interfaces.Infrastructure;
using Dicom;

namespace TheSSS.DICOMViewer.Application.Anonymization.Commands.Handlers;

public class AnonymizeDicomDatasetCommandHandler : IRequestHandler<AnonymizeDicomDatasetCommand, AnonymizationResultDto>
{
    private readonly ISettingsRepositoryAdapter _settingsRepository;
    private readonly IMetadataRepositoryAdapter _metadataRepository;
    private readonly IDicomFileStoreAdapter _fileStore;
    private readonly IAnonymizationRuleEngineAdapter _ruleEngine;
    private readonly IPixelAnonymizationEngineAdapter _pixelEngine;
    private readonly IAuditLogRepositoryAdapter _auditLogger;

    public AnonymizeDicomDatasetCommandHandler(
        ISettingsRepositoryAdapter settingsRepository,
        IMetadataRepositoryAdapter metadataRepository,
        IDicomFileStoreAdapter fileStore,
        IAnonymizationRuleEngineAdapter ruleEngine,
        IPixelAnonymizationEngineAdapter pixelEngine,
        IAuditLogRepositoryAdapter auditLogger)
    {
        _settingsRepository = settingsRepository;
        _metadataRepository = metadataRepository;
        _fileStore = fileStore;
        _ruleEngine = ruleEngine;
        _pixelEngine = pixelEngine;
        _auditLogger = auditLogger;
    }

    public async Task<AnonymizationResultDto> Handle(AnonymizeDicomDatasetCommand request, CancellationToken cancellationToken)
    {
        var result = new AnonymizationResultDto { OriginalSopInstanceUid = request.SopInstanceUid };

        try
        {
            var profile = await ValidateProfile(request.AnonymizationProfileId);
            var instance = await ValidateInstance(request.SopInstanceUid);
            using var originalStream = await GetDicomStream(instance.FilePath);

            var anonymizedStream = await ApplyAnonymization(profile, originalStream);
            var newSopUid = await ProcessAnonymizedData(profile, anonymizedStream, instance);

            result.Success = true;
            result.AnonymizedSopInstanceUid = newSopUid;
            result.Messages.Add("Anonymization completed successfully");
        }
        catch (Exception ex)
        {
            result.Messages.Add($"Anonymization failed: {ex.Message}");
            await _auditLogger.LogEventAsync("ANONYMIZATION_FAILURE", 
                $"Error anonymizing {request.SopInstanceUid}: {ex.Message}", "FAILURE");
        }

        return result;
    }

    private async Task<AnonymizationProfileDto> ValidateProfile(string profileId)
    {
        var profile = await _settingsRepository.GetAnonymizationProfileByIdAsync(profileId);
        return profile ?? throw new NotFoundException(nameof(AnonymizationProfileDto), profileId);
    }

    private async Task<InstanceDto> ValidateInstance(string sopInstanceUid)
    {
        var instance = await _metadataRepository.GetInstanceAsync(sopInstanceUid);
        return instance ?? throw new NotFoundException("DICOM Instance", sopInstanceUid);
    }

    private async Task<Stream> GetDicomStream(string filePath)
    {
        var stream = await _fileStore.GetDicomFileAsync(filePath);
        return stream ?? throw new FileNotFoundException("DICOM file not found", filePath);
    }

    private async Task<Stream> ApplyAnonymization(AnonymizationProfileDto profile, Stream originalStream)
    {
        var processedStream = originalStream;
        
        if (profile.MetadataRules.Count > 0 || !string.IsNullOrEmpty(profile.PredefinedRuleSetName))
        {
            processedStream = await _ruleEngine.ApplyMetadataAnonymizationAsync(processedStream, profile);
        }

        if (profile.PixelAnonymizationTemplateId.HasValue)
        {
            var template = await _settingsRepository.GetPixelAnonymizationTemplateByIdAsync(
                profile.PixelAnonymizationTemplateId.Value) ?? throw new NotFoundException("Pixel Template", profile.PixelAnonymizationTemplateId.Value);
            
            processedStream = await _pixelEngine.ApplyPixelAnonymizationAsync(processedStream, template);
        }

        return processedStream;
    }

    private async Task<string> ProcessAnonymizedData(AnonymizationProfileDto profile, Stream anonymizedStream, InstanceDto originalInstance)
    {
        anonymizedStream.Seek(0, SeekOrigin.Begin);
        var anonymizedFile = await DicomFile.OpenAsync(anonymizedStream);
        var newSopUid = GenerateNewUid(anonymizedFile);

        var storedPath = await _fileStore.StoreDicomFileAsync(
            anonymizedStream,
            anonymizedFile.Dataset.GetString(DicomTag.PatientID),
            anonymizedFile.Dataset.GetString(DicomTag.StudyInstanceUID),
            anonymizedFile.Dataset.GetString(DicomTag.SeriesInstanceUID),
            newSopUid);

        await CreateNewInstanceRecord(anonymizedFile, storedPath, profile.ProfileId);
        await UpdateOriginalInstance(originalInstance, profile.ProfileId);

        await _auditLogger.LogEventAsync("ANONYMIZATION_SUCCESS", 
            $"Created new instance {newSopUid} from {originalInstance.SopInstanceUid}", "SUCCESS");

        return newSopUid;
    }

    private string GenerateNewUid(DicomFile anonymizedFile)
    {
        return anonymizedFile.Dataset.Contains(DicomTag.SOPInstanceUID) 
            ? anonymizedFile.Dataset.GetString(DicomTag.SOPInstanceUID)
            : DicomUIDGenerator.GenerateDerivedFromUUID().UID;
    }

    private async Task CreateNewInstanceRecord(DicomFile anonymizedFile, string storedPath, string profileId)
    {
        var newInstance = new InstanceDto(
            anonymizedFile.Dataset.GetString(DicomTag.SOPInstanceUID),
            anonymizedFile.Dataset.GetString(DicomTag.SeriesInstanceUID),
            anonymizedFile.Dataset.GetString(DicomTag.InstanceNumber),
            anonymizedFile.Dataset.GetString(DicomTag.SOPClassUID),
            storedPath,
            true,
            profileId);

        await _metadataRepository.AddInstanceAsync(newInstance);
    }

    private async Task UpdateOriginalInstance(InstanceDto originalInstance, string profileId)
    {
        await _metadataRepository.UpdateInstanceAnonymizationStatusAsync(
            originalInstance.SopInstanceUid, 
            true,
            profileId);
    }
}