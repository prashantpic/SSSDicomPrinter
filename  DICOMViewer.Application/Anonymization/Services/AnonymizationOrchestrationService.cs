using MediatR;
using System.Collections.Generic;
using System.Threading.Tasks;
using TheSSS.DICOMViewer.Application.Anonymization.Commands;
using TheSSS.DICOMViewer.Application.Anonymization.DTOs;
using TheSSS.DICOMViewer.Application.Anonymization.Interfaces;
using TheSSS.DICOMViewer.Application.Anonymization.Queries;

namespace TheSSS.DICOMViewer.Application.Anonymization.Services;

public class AnonymizationOrchestrationService : IAnonymizationOrchestrationService
{
    private readonly IMediator _mediator;

    public AnonymizationOrchestrationService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<AnonymizationResultDto> AnonymizeDatasetAsync(AnonymizationRequestDto request)
    {
        var command = new AnonymizeDicomDatasetCommand(request.SopInstanceUid, request.AnonymizationProfileId);
        return await _mediator.Send(command);
    }

    public async Task<AnonymizationProfileDto> CreateProfileAsync(AnonymizationProfileDto profileDto)
    {
        var command = new CreateAnonymizationProfileCommand(
            profileDto.ProfileName,
            profileDto.ProfileDescription,
            profileDto.MetadataRules,
            profileDto.PredefinedRuleSetName,
            profileDto.PixelAnonymizationTemplateId,
            profileDto.IsReadOnly);
        
        return await _mediator.Send(command);
    }

    public async Task<AnonymizationProfileDto?> GetProfileByIdAsync(string profileId)
    {
        var query = new GetAnonymizationProfileQuery(profileId);
        return await _mediator.Send(query);
    }

    public async Task<IEnumerable<AnonymizationProfileDto>> GetAllProfilesAsync()
    {
        var query = new GetAllAnonymizationProfilesQuery();
        return await _mediator.Send(query);
    }

    public async Task<AnonymizationProfileDto?> UpdateProfileAsync(AnonymizationProfileDto profileDto)
    {
        var command = new UpdateAnonymizationProfileCommand(
            profileDto.ProfileId,
            profileDto.ProfileName,
            profileDto.ProfileDescription,
            profileDto.MetadataRules,
            profileDto.PredefinedRuleSetName,
            profileDto.PixelAnonymizationTemplateId);
        
        return await _mediator.Send(command);
    }

    public async Task<bool> DeleteProfileAsync(string profileId)
    {
        var command = new DeleteAnonymizationProfileCommand(profileId);
        return await _mediator.Send(command);
    }
}