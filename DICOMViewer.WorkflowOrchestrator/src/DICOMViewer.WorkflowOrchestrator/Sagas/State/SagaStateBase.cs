using System;

namespace TheSSS.DICOMViewer.Application.WorkflowOrchestrator.Sagas.State
{
    public abstract class SagaStateBase
    {
        public Guid WorkflowId { get; set; } = Guid.NewGuid();
        public string CurrentState { get; set; } = "Initialized";
        public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
        public DateTime? LastUpdatedTime { get; set; }
        public DateTime? CompletionTime { get; set; }
        public string Status { get; set; } = "Running";
    }
}