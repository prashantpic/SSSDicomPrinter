using System.Collections.Generic;
using System.Threading.Tasks;
using TheSSS.DICOMViewer.Application.WorkflowOrchestrator.Interfaces;
using TheSSS.DICOMViewer.Application.WorkflowOrchestrator.Services;
using TheSSS.DICOMViewer.Application.WorkflowOrchestrator.Sagas.State;

namespace TheSSS.DICOMViewer.Application.WorkflowOrchestrator.Activities.Network
{
    public class ExecuteCStoreActivity : IWorkflowActivity<ImportWorkflowState>
    {
        private readonly NetworkOperationCoordinator _networkCoordinator;
        private readonly IDicomNetworkServiceAdapter _networkService;

        public ExecuteCStoreActivity(
            NetworkOperationCoordinator networkCoordinator,
            IDicomNetworkServiceAdapter networkService)
        {
            _networkCoordinator = networkCoordinator;
            _networkService = networkService;
        }

        public async Task<bool> ExecuteAsync(ImportWorkflowState state)
        {
            var success = await _networkCoordinator.ExecuteNetworkOperationAsync(async () =>
            {
                foreach (var filePath in state.FilesToProcess)
                {
                    await _networkService.CStoreAsync("DESTINATION_AE", new List<string> { filePath });
                    state.ProcessedFiles.Add(filePath);
                }
                return true;
            }, "C-STORE Operation");

            return success;
        }
    }
}