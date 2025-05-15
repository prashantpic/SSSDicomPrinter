using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TheSSS.DICOMViewer.Application.WorkflowOrchestrator.Activities.Network
{
    public class ExecuteCStoreActivity
    {
        private readonly NetworkOperationCoordinator _networkCoordinator;

        public ExecuteCStoreActivity(NetworkOperationCoordinator networkCoordinator)
        {
            _networkCoordinator = networkCoordinator;
        }

        public async Task ExecuteAsync(Guid pacsNodeId, IEnumerable<string> dicomFiles, Guid workflowId, CancellationToken cancellationToken)
        {
            await _networkCoordinator.SendCStoreAsync(
                pacsNodeId,
                dicomFiles,
                workflowId,
                cancellationToken
            );
        }
    }
}