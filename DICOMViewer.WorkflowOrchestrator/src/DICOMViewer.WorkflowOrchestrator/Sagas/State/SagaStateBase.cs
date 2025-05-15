using System;
using TheSSS.DICOMViewer.Application.WorkflowOrchestrator.Enums;

namespace TheSSS.DICOMViewer.Application.WorkflowOrchestrator.Sagas.State
{
    public abstract class SagaStateBase
    {
        public Guid WorkflowId { get; set; } = Guid.NewGuid();
        public WorkflowStatus Status { get; set; } = WorkflowStatus.NotStarted;
        public string CorrelationId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public int CurrentStep { get; set; }
        public string ErrorDetails { get; set; } = string.Empty;
        public int RetryCount { get; set; }
    }
}