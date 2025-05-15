using MediatR;

namespace TheSSS.DicomViewer.Application.Features.DicomNetwork.CStoreScp;

public record RejectIncomingStudyCommand(string StudyInstanceUid, string Reason) : IRequest;