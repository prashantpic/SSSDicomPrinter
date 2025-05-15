using MediatR;
using TheSSS.DICOMViewer.Application.Anonymization.DTOs;
using TheSSS.DICOMViewer.Domain.Interfaces;
using TheSSS.DICOMViewer.Infrastructure.Interfaces;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace TheSSS.DICOMViewer.Application.Anonymization.Commands.Handlers
{
    public class AnonymizeDicomDatasetCommandHandler : IRequestHandler<AnonymizeDicomDatasetCommand, AnonymizationResultDto>
    {
        private readonly ISettingsRepositoryAdapter _settingsRepository;
        private readonly IMetadataRepositoryAdapter _metadataRepository;
        private readonly IDicomFileStoreAdapter _fileStore;
        private readonly IAnonymizationRuleEngineAdapter _ruleEngine;
        private readonly IPixelAnonymizationEngineAdapter _pixelEngine;
        private readonly IAuditLogRepositoryAdapter _auditLog;

        public AnonymizeDicomDatasetCommandHandler(
            ISettingsRepositoryAdapter settingsRepository,
            IMetadataRepositoryAdapter metadataRepository,
            IDicomFileStoreAdapter fileStore,
            IAnonymizationRuleEngineAdapter ruleEngine,
            IPixelAnonymizationEngineAdapter pixelEngine,
            IAuditLogRepositoryAdapter auditLog)
        {
            _settingsRepository = settingsRepository;
            _metadataRepository = metadataRepository;
            _fileStore = fileStore;
            _ruleEngine = ruleEngine;
            _pixelEngine = pixelEngine;
            _auditLog = auditLog;
        }

        public async Task<AnonymizationResultDto> Handle(AnonymizeDicomDatasetCommand request, CancellationToken cancellationToken)
        {
            var profile = await _settingsRepository.GetAnonymizationProfileByIdAsync(request.AnonymizationProfileId)
                ?? throw new NotFoundException("Anonymization profile not found");

            var instance = await _metadataRepository.GetInstanceAsync(request.SopInstanceUid)
                ?? throw new NotFoundException("DICOM instance not found");

            var originalPath = await _fileStore.GetDicomFilePathAsync(request.SopInstanceUid);
            using var originalStream = await _fileStore.GetDicomFileAsync(originalPath);

            var workingStream = originalStream;
            
            if (profile.MetadataAnonymizationRules != null)
            {
                workingStream = await _ruleEngine.ApplyMetadataAnonymizationAsync(workingStream, profile);
            }

            if (profile.PixelAnonymizationTemplateId.HasValue)
            {
                var template = await _settingsRepository.GetPixelAnonymizationTemplateByIdAsync(profile.PixelAnonymizationTemplateId.Value);
                workingStream = await _pixelEngine.ApplyPixelAnonymizationAsync(workingStream, template);
            }

            var newSopUid = ShouldGenerateNewSopUid(profile, workingStream != originalStream) 
                ? GenerateNewSopUid() 
                : request.SopInstanceUid;

            var storagePath = await _fileStore.StoreDicomFileAsync(
                await GetStreamContent(workingStream),
                instance.PatientId,
                instance.StudyInstanceUid,
                instance.SeriesInstanceUid,
                newSopUid);

            await _metadataRepository.UpdateInstanceAnonymizationStatusAsync(
                request.SopInstanceUid,
                true,
                profile.ProfileId);

            if (newSopUid != request.SopInstanceUid)
            {
                await _metadataRepository.AddInstanceAsync(new InstanceDto(
                    newSopUid,
                    instance.SeriesInstanceUid,
                    instance.SopClassUid,
                    instance.InstanceNumber,
                    storagePath,
                    instance.PhotometricInterpretation,
                    instance.Rows,
                    instance.Columns,
                    true,
                    profile.ProfileId));
            }

            await _auditLog.LogEventAsync("AnonymizationComplete", 
                $"Anonymized SOP Instance UID: {request.SopInstanceUid} => {newSopUid} using profile {profile.ProfileName}",
                "Success");

            return new AnonymizationResultDto(
                true,
                request.SopInstanceUid,
                newSopUid,
                new List<string> { "Anonymization completed successfully" });
        }

        private bool ShouldGenerateNewSopUid(AnonymizationProfileDto profile, bool pixelDataModified)
        {
            return pixelDataModified || profile.AlwaysGenerateNewSopUid;
        }

        private string GenerateNewSopUid()
        {
            return Dicom.UID.Generate().ToString();
        }

        private async Task<byte[]> GetStreamContent(Stream stream)
        {
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            return memoryStream.ToArray();
        }
    }
}