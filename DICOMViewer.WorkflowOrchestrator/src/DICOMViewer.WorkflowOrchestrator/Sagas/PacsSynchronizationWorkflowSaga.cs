using MediatR;
using TheSSS.DICOMViewer.Application.WorkflowOrchestrator.Activities;
using TheSSS.DICOMViewer.Application.WorkflowOrchestrator.Contracts.Events;
using TheSSS.DICOMViewer.Application.WorkflowOrchestrator.Exceptions;
using TheSSS.DICOMViewer.Application.WorkflowOrchestrator.Interfaces;
using TheSSS.DICOMViewer.Application.WorkflowOrchestrator.Sagas.State;
using Microsoft.Extensions.Logging;

namespace TheSSS.DICOMViewer.Application.WorkflowOrchestrator.Sagas;

public class PacsSynchronizationWorkflowSaga : IRequestHandler<StartPacsSynchronizationWorkflowCommand>
{
    private readonly IWorkflowStateRepository _stateRepository;
    private readonly IMediator _mediator;
    private readonly IDicomNetworkServiceAdapter _networkService;
    private readonly ILogger<PacsSynchronizationWorkflowSaga> _logger;

    public PacsSynchronizationWorkflowSaga(
        IWorkflowStateRepository stateRepository,
        IMediator mediator,
        IDicomNetworkServiceAdapter networkService,
        ILogger<PacsSynchronizationWorkflowSaga> logger)
    {
        _stateRepository = stateRepository;
        _mediator = mediator;
        _networkService = networkService;
        _logger = logger;
    }

    public async Task Handle(StartPacsSynchronizationWorkflowCommand request, CancellationToken cancellationToken)
    {
        var workflowId = Guid.NewGuid().ToString();
        var initialState = new PacsSynchronizationWorkflowState
        {
            WorkflowId = workflowId,
            PacsNode = request.PacsNode,
            QueryParameters = request.QueryParameters,
            Status = WorkflowStatus.Running,
            StartTime = DateTime.UtcNow
        };

        try
        {
            await _stateRepository.SaveStateAsync(workflowId, initialState);
            await _mediator.Publish(new WorkflowStartedEvent(workflowId), cancellationToken);

            var activities = new IWorkflowActivity<PacsSynchronizationWorkflowState>[] {
                new ExecuteCFindActivity(_networkService),
                new ProcessCFindResultsActivity(),
                new ExecuteCMoveOrCStoreActivity(_networkService)
            };

            foreach (var activity in activities)
            {
                initialState = await ExecuteActivity(initialState, activity, cancellationToken);
            }

            await CompleteWorkflow(initialState);
        }
        catch (Exception ex)
        {
            await HandleWorkflowFailure(initialState, ex);
        }
    }

    private async Task<PacsSynchronizationWorkflowState> ExecuteActivity(
        PacsSynchronizationWorkflowState state,
        IWorkflowActivity<PacsSynchronizationWorkflowState> activity,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await activity.ExecuteAsync(state, cancellationToken);
            state.CurrentStep = activity.GetType().Name;
            state.LastUpdated = DateTime.UtcNow;
            await _stateRepository.SaveStateAsync(state.WorkflowId, state);
            return state;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Activity {Activity} failed in workflow {WorkflowId}", activity.GetType().Name, state.WorkflowId);
            throw new WorkflowExecutionException($"Activity {activity.GetType().Name} failed", ex);
        }
    }

    private async Task CompleteWorkflow(PacsSynchronizationWorkflowState state)
    {
        state.Status = WorkflowStatus.Completed;
        state.CompletionTime = DateTime.UtcNow;
        await _stateRepository.SaveStateAsync(state.Workflow