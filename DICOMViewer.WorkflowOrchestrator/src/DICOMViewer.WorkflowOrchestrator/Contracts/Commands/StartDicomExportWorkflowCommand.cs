using MediatR;

namespace TheSSS.DICOMViewer.Application.WorkflowOrchestrator.Contracts.Commands
{
    public class StartDicomExportWorkflowCommand : IRequest
    {
        public required string[] SopInstanceUids { get; init; }
        public required int DestinationPacsNodeId { get; init; }
        public bool AnonymizeBeforeExport { get; set; }
    }
}