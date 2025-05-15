using System.Collections.Generic;
using TheSSS.DICOMViewer.Application.WorkflowOrchestrator.Enums;

namespace TheSSS.DICOMViewer.Application.WorkflowOrchestrator.Sagas.State
{
    public class ImportWorkflowState : SagaStateBase
    {
        public List<string> FilesToProcess { get; set; } = new();
        public List<string> ProcessedFiles { get; set; } = new();
        public Dictionary<string, string> ErroredFiles { get; set; } = new();
        public string SourcePath { get; set; } = string.Empty;
        public int ParallelProcessingLimit { get; set; } = 1;
        public ResourceConstraints ResourceConstraints { get; set; } = new();
        public Dictionary<string, object> ImportOptions { get; set; } = new();
    }

    public class ResourceConstraints
    {
        public int MaxConcurrentImports { get; set; } = 4;
        public int MemoryLimitMB { get; set; } = 4096;
    }
}