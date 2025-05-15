using MediatR;
using TheSSS.DICOMViewer.Application.Anonymization.DTOs;
using TheSSS.DICOMViewer.Infrastructure.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace TheSSS.DICOMViewer.Application.Anonymization.Commands.Handlers
{
    public class UpdateAnonymizationProfileCommandHandler : IRequestHandler<UpdateAnonymizationProfileCommand, AnonymizationProfileDto>
    {
        private readonly ISettingsRepositoryAdapter _settingsRepository;
        private readonly IAuditLogRepositoryAdapter _auditLog;

        public UpdateAnonymizationProfileCommandHandler(
            ISettingsRepositoryAdapter settingsRepository,
            IAuditLogRepositoryAdapter auditLog)
        {
            _settingsRepository = settingsRepository;
            _auditLog = auditLog;
        }

        public async Task<AnonymizationProfileDto> Handle(UpdateAnonymizationProfileCommand request, CancellationToken cancellationToken)
        {
            var existingProfile = await _settingsRepository.GetAnonymizationProfileByIdAsync(request.ProfileId)
                ?? throw new NotFoundException($"Profile with ID {request.ProfileId} not found");

            if (existingProfile.IsReadOnly)
                throw new ValidationException("Read-only profiles cannot be modified");

            var updatedProfile = new AnonymizationProfileDto(
                request.ProfileId,
                request.ProfileName,
                request.ProfileDescription,
                request.MetadataRules,
                existingProfile.PixelAnonymizationTemplateId,
                existingProfile.IsReadOnly);

            var result = await _settingsRepository.UpdateAnonymizationProfileAsync(updatedProfile);

            await _auditLog.LogEventAsync("ProfileUpdated", 
                $"Updated anonymization profile: {result.ProfileName} (ID: {result.ProfileId})",
                "Success");

            return result;
        }
    }
}