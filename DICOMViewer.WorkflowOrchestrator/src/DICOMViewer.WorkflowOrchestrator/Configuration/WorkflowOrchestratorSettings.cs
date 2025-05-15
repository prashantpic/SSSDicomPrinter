using System;

namespace TheSSS.DICOMViewer.Application.WorkflowOrchestrator.Configuration
{
    public class WorkflowOrchestratorSettings
    {
        public int NetworkRetryCount { get; set; } = 3;
        public int NetworkRetryDelaySeconds { get; set; } = 5;
        public int MaxConcurrentImports { get; set; } = 4;
        public int MaxConcurrentNetworkOperations { get; set; } = 10;
        public string SagaStatePersistencePath { get; set; } = "./workflow_states";
    }
}