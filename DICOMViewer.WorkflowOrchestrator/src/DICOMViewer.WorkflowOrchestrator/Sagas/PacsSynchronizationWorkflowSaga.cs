using MediatR;
using TheSSS.DICOMViewer.Application.WorkflowOrchestrator.Interfaces;

namespace TheSSS.DICOMViewer.Application.WorkflowOrchestrator.Sagas;

public class PacsSynchronizationWorkflowSaga
{
    private readonly IWorkflowStateRepository _stateRepository;
    private readonly NetworkOperationCoordinator _networkCoordinator;
    private readonly IMediator _mediator;

    public PacsSynchronizationWorkflowSaga(
        IWorkflowStateRepository stateRepository,
        NetworkOperationCoordinator networkCoordinator,
        IMediator mediator)
    {
        _stateRepository = stateRepository;
        _networkCoordinator = networkCoordinator;
        _mediator = mediator;
    }

    public async Task HandleStartCommand(StartPacsSynchronizationWorkflowCommand command)
    {
        var state = new PacsSyncWorkflowState
        {
            WorkflowId = command.WorkflowId,
            PacsNodeId = command.PacsNodeId,
            SyncType = command.SyncType,
            Status = "Initializing"
        };

        await _stateRepository.SaveStateAsync(state);
        await PerformSyncOperation(state);
    }

    private async Task PerformSyncOperation(PacsSyncWorkflowState state)
    {
        state.Status = "Syncing";
        await _stateRepository.SaveStateAsync(state);

        try
        {
            // Implementation would include actual sync logic
            await _networkCoordinator.PerformCFindAsync(
                state.PacsNodeId,
                new QueryParameters(),
                state.WorkflowId,
                CancellationToken.None);

            state.Status = "Completed";
            await _mediator.Publish(new WorkflowCompletedEvent(state.WorkflowId, DateTime.UtcNow));
        }
        catch (Exception ex)
        {
            state.Status = "Failed";
            await _mediator.Publish(new WorkflowFailedEvent(
                state.WorkflowId,
                DateTime.UtcNow,
                ex.Message,
                ex.ToString(),
                "PacsSync"));
            throw;
        }
        finally
        {
            await _stateRepository.SaveStateAsync(state);
        }
    }
}