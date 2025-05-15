namespace TheSSS.DICOMViewer.Application.WorkflowOrchestrator.Interfaces
{
    public interface IWorkflowProgressReporter
    {
        void ReportProgress(string workflowId, int current, int total, string message);
    }
}