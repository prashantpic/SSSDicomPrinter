using MediatR;

namespace TheSSS.DicomViewer.Application.Features.DicomNetwork.CStoreScp
{
    public record RejectIncomingStudyCommand(
        string StudyInstanceUidOrPath,
        string Reason) : IRequest<Unit>;
}