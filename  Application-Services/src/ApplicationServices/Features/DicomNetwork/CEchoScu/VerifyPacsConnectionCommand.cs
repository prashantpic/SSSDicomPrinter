using MediatR;
using TheSSS.DicomViewer.Application.DTOs.Pacs;

namespace TheSSS.DicomViewer.Application.Features.DicomNetwork.CEchoScu
{
    public record VerifyPacsConnectionCommand(int PacsNodeId) : IRequest<CEchoResultDto>;
}