using System.Threading;
using System.Threading.Tasks;
using TheSSS.DICOMViewer.Application.WorkflowOrchestrator.Interfaces;

namespace TheSSS.DICOMViewer.Application.WorkflowOrchestrator.Activities.DicomImport
{
    public class ValidateDicomFilesActivity
    {
        private readonly IDicomImportServiceAdapter _importService;
        private readonly IWorkflowProgressReporter _progressReporter;

        public ValidateDicomFilesActivity(IDicomImportServiceAdapter importService, IWorkflowProgressReporter progressReporter)
        {
            _importService = importService;
            _progressReporter = progressReporter;
        }

        public async Task ExecuteAsync(ImportWorkflowState state, CancellationToken cancellationToken)
        {
            state.CurrentActivity = "Validation";
            foreach (var filePath in state.FilesToProcess)
            {
                var result = await _importService.ValidateFileAsync(filePath, state.WorkflowId, cancellationToken);
                
                if (result.IsValid)
                {
                    state.ProcessedFiles.Add(filePath);
                }
                else
                {
                    state.FailedFiles.Add(filePath);
                }

                state.ProcessedFilesCount++;
                await _progressReporter.ReportProgressAsync(
                    state.WorkflowId,
                    (int)((double)state.ProcessedFilesCount / state.TotalFilesCount * 100),
                    $"Validated {filePath}",
                    "Validation",
                    state.ProcessedFilesCount,
                    state.TotalFilesCount
                );
            }
        }
    }
}