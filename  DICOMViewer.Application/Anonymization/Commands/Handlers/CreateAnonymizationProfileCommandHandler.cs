using MediatR;
using System.Threading;
using System.Threading.Tasks;
using TheSSS.DICOMViewer.Application.Anonymization.Commands;
using TheSSS.DICOMViewer.Application.Anonymization.DTOs;
using TheSSS.DICOMViewer.Application.Interfaces.Infrastructure;

namespace TheSSS.DICOMViewer.Application.Anonymization.Commands.Handlers;

public class CreateAnonymizationProfileCommandHandler : IRequestHandler<CreateAnonymizationProfileCommand, AnonymizationProfileDto>
{
    private readonly ISettingsRepositoryAdapter _settingsRepository;
    private readonly IAuditLogRepositoryAdapter _auditLogger;

    public CreateAnonymizationProfileCommandHandler(
        ISettingsRepositoryAdapter settingsRepository,
        IAuditLogRepositoryAdapter auditLogger)
    {
        _settingsRepository = settingsRepository;
        _auditLogger = auditLogger;
    }

    public async Task<AnonymizationProfileDto> Handle(CreateAnonymizationProfileCommand request, CancellationToken cancellationToken)
    {
        var profile = new AnonymizationProfileDto
        {
            ProfileId = Guid.NewGuid().ToString(),
            ProfileName = request.ProfileName,
            ProfileDescription = request.ProfileDescription,
            MetadataRules = request.MetadataRules,
            PredefinedRuleSetName = request.PredefinedRuleSetName,
            PixelAnonymizationTemplateId = request.PixelAnonymizationTemplateId,
            IsReadOnly = request.IsReadOnly
        };

        var createdProfile = await _settingsRepository.CreateAnonymizationProfileAsync(profile);
        
        await _auditLogger.LogEventAsync("PROFILE_CREATED", 
            $"Created profile: {createdProfile.ProfileName} ({createdProfile.ProfileId})", 
            "SUCCESS");

        return createdProfile;
    }
}