using MediatR;

namespace TheSSS.DICOMViewer.Application.WorkflowOrchestrator.Contracts.Events
{
    public class WorkflowCompletedEvent : INotification
    {
        public required Guid WorkflowId { get; init; }
        public DateTime CompletionTime { get; init; }
    }
}