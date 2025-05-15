using MediatR;
using TheSSS.DICOMViewer.Application.Anonymization.DTOs;
using TheSSS.DICOMViewer.Infrastructure.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace TheSSS.DICOMViewer.Application.Anonymization.Commands.Handlers
{
    public class CreateAnonymizationProfileCommandHandler : IRequestHandler<CreateAnonymizationProfileCommand, AnonymizationProfileDto>
    {
        private readonly ISettingsRepositoryAdapter _settingsRepository;
        private readonly IAuditLogRepositoryAdapter _auditLog;

        public CreateAnonymizationProfileCommandHandler(
            ISettingsRepositoryAdapter settingsRepository,
            IAuditLogRepositoryAdapter auditLog)
        {
            _settingsRepository = settingsRepository;
            _auditLog = auditLog;
        }

        public async Task<AnonymizationProfileDto> Handle(CreateAnonymizationProfileCommand request, CancellationToken cancellationToken)
        {
            var profileDto = new AnonymizationProfileDto(
                0,
                request.ProfileName,
                request.ProfileDescription,
                request.MetadataRules,
                request.PixelAnonymizationTemplateId,
                false);

            var createdProfile = await _settingsRepository.CreateAnonymizationProfileAsync(profileDto);

            await _auditLog.LogEventAsync("ProfileCreated", 
                $"Created anonymization profile: {createdProfile.ProfileName} (ID: {createdProfile.ProfileId})",
                "Success");

            return createdProfile;
        }
    }
}