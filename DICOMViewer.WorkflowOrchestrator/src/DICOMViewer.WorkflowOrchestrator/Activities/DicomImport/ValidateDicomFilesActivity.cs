using System.Threading.Tasks;
using TheSSS.DICOMViewer.Application.WorkflowOrchestrator.Interfaces;
using TheSSS.DICOMViewer.Application.WorkflowOrchestrator.Sagas.State;

namespace TheSSS.DICOMViewer.Application.WorkflowOrchestrator.Activities.DicomImport
{
    public class ValidateDicomFilesActivity : IWorkflowActivity<ImportWorkflowState>
    {
        private readonly IDicomImportServiceAdapter _importService;

        public ValidateDicomFilesActivity(IDicomImportServiceAdapter importService)
        {
            _importService = importService;
        }

        public async Task<bool> ExecuteAsync(ImportWorkflowState state)
        {
            foreach (var filePath in state.FilesToProcess)
            {
                try
                {
                    var isValid = await _importService.ValidateDicomFileAsync(filePath);
                    if (!isValid)
                    {
                        state.ErroredFiles[filePath] = "Invalid DICOM file format";
                    }
                }
                catch (Exception ex)
                {
                    state.ErroredFiles[filePath] = $"Validation error: {ex.Message}";
                }
            }
            return state.ErroredFiles.Count == 0;
        }
    }
}