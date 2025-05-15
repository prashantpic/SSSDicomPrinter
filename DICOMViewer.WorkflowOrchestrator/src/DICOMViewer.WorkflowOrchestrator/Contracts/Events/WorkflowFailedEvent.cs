using MediatR;

namespace TheSSS.DICOMViewer.Application.WorkflowOrchestrator.Contracts.Events
{
    public class WorkflowFailedEvent : INotification
    {
        public required Guid WorkflowId { get; init; }
        public required DateTime FailureTime { get; init; }
        public required string ErrorDetails { get; init; }
        public string? FailedStep { get; init; }
    }
}