using MediatR;
using TheSSS.DICOMViewer.Application.WorkflowOrchestrator.Activities;
using TheSSS.DICOMViewer.Application.WorkflowOrchestrator.Contracts.Events;
using TheSSS.DICOMViewer.Application.WorkflowOrchestrator.Exceptions;
using TheSSS.DICOMViewer.Application.WorkflowOrchestrator.Interfaces;
using TheSSS.DICOMViewer.Application.WorkflowOrchestrator.Sagas.State;
using Microsoft.Extensions.Logging;

namespace TheSSS.DICOMViewer.Application.WorkflowOrchestrator.Sagas;

public class AnonymizeDicomWorkflowSaga : IRequestHandler<StartDicomAnonymizationWorkflowCommand>
{
    private readonly IWorkflowStateRepository _stateRepository;
    private readonly IMediator _mediator;
    private readonly IAnonymizationServiceAdapter _anonymizationService;
    private readonly IAuditLoggerAdapter _auditLogger;
    private readonly ILogger<AnonymizeDicomWorkflowSaga> _logger;

    public AnonymizeDicomWorkflowSaga(
        IWorkflowStateRepository stateRepository,
        IMediator mediator,
        IAnonymizationServiceAdapter anonymizationService,
        IAuditLoggerAdapter auditLogger,
        ILogger<AnonymizeDicomWorkflowSaga> logger)
    {
        _stateRepository = stateRepository;
        _mediator = mediator;
        _anonymizationService = anonymizationService;
        _auditLogger = auditLogger;
        _logger = logger;
    }

    public async Task Handle(StartDicomAnonymizationWorkflowCommand request, CancellationToken cancellationToken)
    {
        var workflowId = Guid.NewGuid().ToString();
        var initialState = new AnonymizationWorkflowState
        {
            WorkflowId = workflowId,
            SopInstanceUids = request.SopInstanceUids,
            AnonymizationProfileId = request.AnonymizationProfileId,
            Status = WorkflowStatus.Running,
            StartTime = DateTime.UtcNow
        };

        try
        {
            await _stateRepository.SaveStateAsync(workflowId, initialState);
            await _mediator.Publish(new WorkflowStartedEvent(workflowId), cancellationToken);

            var activities = new IWorkflowActivity<AnonymizationWorkflowState>[] {
                new LoadDicomDatasetActivity(_anonymizationService),
                new ApplyAnonymizationActivity(_anonymizationService),
                new SaveAnonymizedDatasetActivity(_anonymizationService),
                new LogAnonymizationActivity(_auditLogger)
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

    private async Task<AnonymizationWorkflowState> ExecuteActivity(
        AnonymizationWorkflowState state,
        IWorkflowActivity<AnonymizationWorkflowState> activity,
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

    private async Task CompleteWorkflow(AnonymizationWorkflowState state)
    {
        state.Status = WorkflowStatus.Completed;
        state.CompletionTime = DateTime.UtcNow;
        await _stateRepository.SaveStateAsync(state.WorkflowId, state);
        await _mediator.Publish(new WorkflowCompletedEvent(state.WorkflowId, state.CompletionTime.Value));
    }

    private async Task HandleWorkflowFailure(AnonymizationWorkflowState state, Exception ex)
    {
        state.Status = WorkflowStatus.Failed;
        state.ErrorDetails = ex.ToString();
        state.CompletionTime = DateTime.UtcNow;
        
        await _stateRepository.SaveStateAsync(state.WorkflowId, state);
        await _mediator.Publish(new WorkflowFailedEvent(state.WorkflowId, ex));
        await CompensateFailedWorkflow(state);
    }

    private async Task CompensateFailedWorkflow(AnonymizationWorkflowState state)
    {
        try
        {
            if (state.AnonymizedInstances.Any())
            {
                await _anonymizationService.RollbackAnonymizationAsync(state.OriginalInstances);
            }
            _logger.LogInformation("Compensation completed for workflow {WorkflowId}", state.WorkflowId);
        }
        catch (Exception compensationEx)
        {
            _logger.LogError(compensationEx, "Compensation failed for workflow {WorkflowId}", state.WorkflowId);
        }
    }
}