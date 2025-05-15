using MediatR;
using TheSSS.DICOMViewer.Application.WorkflowOrchestrator.Activities;
using TheSSS.DICOMViewer.Application.WorkflowOrchestrator.Contracts.Events;
using TheSSS.DICOMViewer.Application.WorkflowOrchestrator.Exceptions;
using TheSSS.DICOMViewer.Application.WorkflowOrchestrator.Interfaces;
using TheSSS.DICOMViewer.Application.WorkflowOrchestrator.Sagas.State;
using Microsoft.Extensions.Logging;

namespace TheSSS.DICOMViewer.Application.WorkflowOrchestrator.Sagas;

public class DicomExportWorkflowSaga : IRequestHandler<StartDicomExportWorkflowCommand>
{
    private readonly IWorkflowStateRepository _stateRepository;
    private readonly IMediator _mediator;
    private readonly IDicomNetworkServiceAdapter _networkService;
    private readonly ILogger<DicomExportWorkflowSaga> _logger;

    public DicomExportWorkflowSaga(
        IWorkflowStateRepository stateRepository,
        IMediator mediator,
        IDicomNetworkServiceAdapter networkService,
        ILogger<DicomExportWorkflowSaga> logger)
    {
        _stateRepository = stateRepository;
        _mediator = mediator;
        _networkService = networkService;
        _logger = logger;
    }

    public async Task Handle(StartDicomExportWorkflowCommand request, CancellationToken cancellationToken)
    {
        var workflowId = Guid.NewGuid().ToString();
        var initialState = new ExportWorkflowState
        {
            WorkflowId = workflowId,
            SopInstanceUids = request.SopInstanceUids,
            DestinationPacsNode = request.DestinationPacsNode,
            Status = WorkflowStatus.Running,
            StartTime = DateTime.UtcNow
        };

        try
        {
            await _stateRepository.SaveStateAsync(workflowId, initialState);
            await _mediator.Publish(new WorkflowStartedEvent(workflowId), cancellationToken);

            var activities = new IWorkflowActivity<ExportWorkflowState>[] {
                new PrepareExportDatasetActivity(),
                new ExecuteCStoreActivity(_networkService)
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

    private async Task<ExportWorkflowState> ExecuteActivity(
        ExportWorkflowState state,
        IWorkflowActivity<ExportWorkflowState> activity,
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

    private async Task CompleteWorkflow(ExportWorkflowState state)
    {
        state.Status = WorkflowStatus.Completed;
        state.CompletionTime = DateTime.UtcNow;
        await _stateRepository.SaveStateAsync(state.WorkflowId, state);
        await _mediator.Publish(new WorkflowCompletedEvent(state.WorkflowId, state.CompletionTime.Value));
    }

    private async Task HandleWorkflowFailure(ExportWorkflowState state, Exception ex)
    {
        state.Status = WorkflowStatus.Failed;
        state.ErrorDetails = ex.ToString();
        state.CompletionTime = DateTime.UtcNow;
        
        await _stateRepository.SaveStateAsync(state.WorkflowId, state);
        await _mediator.Publish(new WorkflowFailedEvent(state.WorkflowId, ex));
    }
}