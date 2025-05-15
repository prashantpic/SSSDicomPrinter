using MediatR;
using TheSSS.DICOMViewer.Application.DicomNetwork.Commands;
using TheSSS.DICOMViewer.Application.DicomNetwork.DTOs;

namespace TheSSS.DICOMViewer.Application.DicomNetwork.Services;

public class DicomNetworkService : IDicomNetworkService
{
    private readonly IMediator _mediator;

    public DicomNetworkService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<DicomNetworkResultDto> VerifyPacsConnectivityAsync(string pacsNodeId)
    {
        var command = new VerifyPacsConnectivityCommand(pacsNodeId);
        return await _mediator.Send(command);
    }
}