using MediatR;
using TheSSS.DICOMViewer.Infrastructure.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace TheSSS.DICOMViewer.Application.Anonymization.Commands.Handlers
{
    public class DeleteAnonymizationProfileCommandHandler : IRequestHandler<DeleteAnonymizationProfileCommand, bool>
    {
        private readonly ISettingsRepositoryAdapter _settingsRepository;
        private readonly IAuditLogRepositoryAdapter _auditLog;

        public DeleteAnonymizationProfileCommandHandler(
            ISettingsRepositoryAdapter settingsRepository,
            IAuditLogRepositoryAdapter auditLog)
        {
            _settingsRepository = settingsRepository;
            _auditLog = auditLog;
        }

        public async Task<bool> Handle(DeleteAnonymizationProfileCommand request, CancellationToken cancellationToken)
        {
            var profile = await _settingsRepository.GetAnonymizationProfileByIdAsync(request.ProfileId)
                ?? throw new NotFoundException($"Profile with ID {request.ProfileId} not found");

            if (profile.IsReadOnly)
                throw new ValidationException("Read-only profiles cannot be deleted");

            var success = await _settingsRepository.DeleteAnonymizationProfileAsync(request.ProfileId);

            if (success)
            {
                await _auditLog.LogEventAsync("ProfileDeleted", 
                    $"Deleted anonymization profile: {profile.ProfileName} (ID: {request.ProfileId})",
                    "Success");
            }

            return success;
        }
    }
}