using MediatR;
using TheSSS.DicomViewer.Application.Features.Anonymization.Commands;
using TheSSS.DicomViewer.Application.Interfaces.Application;

namespace TheSSS.DicomViewer.Application.Services;

public class AnonymizationOrchestrationService : IAnonymizationOrchestrationService
{
    private readonly IMediator _mediator;

    public AnonymizationOrchestrationService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<AnonymizationResultDto> ApplyProfileToDatasetAsync(string sopInstanceUid, int profileId, CancellationToken cancellationToken = default)
    {
        var command = new ApplyAnonymizationProfileCommand(sopInstanceUid, profileId);
        return await _mediator.Send(command, cancellationToken);
    }

    public async Task<int> CreateAnonymizationProfileAsync(AnonymizationProfileDto profileDto, CancellationToken cancellationToken = default)
    {
        var command = new CreateAnonymizationProfileCommand(profileDto);
        return await _mediator.Send(command, cancellationToken);
    }
}