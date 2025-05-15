using System.Collections.Generic;

namespace TheSSS.DICOMViewer.Application.WorkflowOrchestrator.Sagas.State
{
    public class ImportWorkflowState : SagaStateBase
    {
        public string SourcePath { get; set; }
        public List<string> FilesToProcess { get; set; } = new List<string>();
        public List<string> ProcessedFiles { get; set; } = new List<string>();
        public List<string> FailedFiles { get; set; } = new List<string>();
        public string CurrentFileBeingProcessed { get; set; }
        public int TotalFilesCount { get; set; }
        public int ProcessedFilesCount { get; set; }
        public string CurrentActivity { get; set; }
        public ImportOptions Options { get; set; } = new ImportOptions();
    }

    public class ImportOptions
    {
        public bool Recursive { get; set; }
        public bool ValidateFiles { get; set; } = true;
        public int MaxParallelism { get; set; } = 4;
    }
}