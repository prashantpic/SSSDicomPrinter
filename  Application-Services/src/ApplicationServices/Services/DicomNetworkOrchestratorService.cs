using MediatR;
using TheSSS.DicomViewer.Application.Features.DicomNetwork.CEchoScu.Commands;
using TheSSS.DicomViewer.Application.Features.DicomNetwork.CStoreScu.Commands;
using TheSSS.DicomViewer.Application.Interfaces.Application;

namespace TheSSS.DicomViewer.Application.Services;

public class DicomNetworkOrchestratorService : IDicomNetworkService
{
    private readonly IMediator _mediator;

    public DicomNetworkOrchestratorService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<CEchoResultDto> VerifyPacsConnectionAsync(int pacsNodeId, CancellationToken cancellationToken = default)
    {
        var command = new VerifyPacsConnectionCommand(pacsNodeId);
        return await _mediator.Send(command, cancellationToken);
    }

    public async Task<CStoreScuResultDto> SendInstancesToPacsAsync(int pacsNodeId, IEnumerable<string> sopInstanceUids, CancellationToken cancellationToken = default)
    {
        var command = new SendDicomInstancesToPacsCommand(pacsNodeId, sopInstanceUids);
        return await _mediator.Send(command, cancellationToken);
    }
}