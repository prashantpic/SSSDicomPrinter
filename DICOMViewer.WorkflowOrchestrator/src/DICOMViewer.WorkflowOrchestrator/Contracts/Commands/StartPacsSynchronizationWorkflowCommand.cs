using MediatR;

namespace TheSSS.DICOMViewer.Application.WorkflowOrchestrator.Contracts.Commands
{
    public class StartPacsSynchronizationWorkflowCommand : IRequest
    {
        public required int PacsNodeId { get; init; }
        public required DateTime StartDate { get; init; }
        public required DateTime EndDate { get; init; }
        public string[] Modalities { get; set; } = Array.Empty<string>();
    }
}