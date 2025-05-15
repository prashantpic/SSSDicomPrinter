using MediatR;

namespace TheSSS.DICOMViewer.Application.WorkflowOrchestrator.Contracts.Commands
{
    public class StartDicomAnonymizationWorkflowCommand : IRequest
    {
        public required string[] SopInstanceUids { get; init; }
        public required int AnonymizationProfileId { get; init; }
        public bool CreateNewVersion { get; set; } = true;
    }
}