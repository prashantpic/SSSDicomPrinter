using MediatR;

namespace TheSSS.DICOMViewer.Application.WorkflowOrchestrator.Contracts.Events
{
    public class WorkflowProgressUpdatedEvent : INotification
    {
        public required Guid WorkflowId { get; init; }
        public int ProcessedItems { get; init; }
        public int TotalItems { get; init; }
        public string CurrentStepDescription { get; init; } = string.Empty;
    }
}