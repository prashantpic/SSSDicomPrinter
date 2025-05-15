using MediatR;
using TheSSS.DICOMViewer.Application.WorkflowOrchestrator.Activities;
using TheSSS.DICOMViewer.Application.WorkflowOrchestrator.Contracts.Events;
using TheSSS.DICOMViewer.Application.WorkflowOrchestrator.Exceptions;
using TheSSS.DICOMViewer.Application.WorkflowOrchestrator.Interfaces;
using TheSSS.DICOMViewer.Application.WorkflowOrchestrator.Sagas.State;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace TheSSS.DICOMViewer.Application.WorkflowOrchestrator.Sagas;

public class ImportDicomWorkflowSaga : IRequestHandler<StartDicomImportWorkflowCommand>
{
    private readonly IWorkflowStateRepository _stateRepository;
    private readonly IMediator _mediator;
    private readonly IResourceGovernor _resourceGovernor;
    private readonly IWorkflowProgressReporter _progressReporter;
    private readonly IDicomImportServiceAdapter _importService;
    private readonly IAuditLoggerAdapter _auditLogger;
    private readonly ILogger<ImportDicomWorkflowSaga> _logger;
    private readonly WorkflowOrchestratorSettings _settings;

    public ImportDicomWorkflowSaga(
        IWorkflowStateRepository stateRepository,
        IMediator mediator,
        IResourceGovernor resourceGovernor,
        IWorkflowProgressReporter progressReporter,
        IDicomImportServiceAdapter importService,
        IAuditLoggerAdapter auditLogger,
        ILogger<ImportDicomWorkflowSaga> logger,
        IOptions<WorkflowOrchestratorSettings> settings)
    {
        _stateRepository = stateRepository;
        _mediator = mediator;
        _resourceGovernor = resourceGovernor;
        _progressReporter = progressReporter;
        _importService = importService;
        _auditLogger = auditLogger;
        _logger = logger;
        _settings = settings.Value;
    }

    public async Task Handle(StartDicomImportWorkflowCommand request, CancellationToken cancellationToken)
    {
        var workflowId = Guid.NewGuid().ToString();
        var initialState = new ImportWorkflowState
        {
            WorkflowId = workflowId,
            Status = WorkflowStatus.Running,
            SourcePath = request.SourcePath,
            ImportOptions = request.ImportOptions,
            StartTime = DateTime.UtcNow
        };

        try
        {
            await _stateRepository.SaveStateAsync(workflowId, initialState);
            await _mediator.Publish(new WorkflowStartedEvent(workflowId), cancellationToken);

            var activities = new IWorkflowActivity<ImportWorkflowState>[] {
                new ValidateDicomFilesActivity(_importService, _logger),
                new CheckResourceConstraintsActivity(_resourceGovernor, _settings),
                new ImportFilesActivity(_importService, _progressReporter, _logger),
                new UpdateDatabaseActivity(_importService),
                new ReportProgressActivity(_progressReporter)
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

    private async Task<ImportWorkflowState> ExecuteActivity(
        ImportWorkflowState state,
        IWorkflowActivity<ImportWorkflowState> activity,
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
        catch (ResourceConstraintViolationException rcvEx)
        {
            _logger.LogWarning(rcvEx, "Resource constraints violated for workflow {WorkflowId}", state.WorkflowId);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Activity {Activity} failed in workflow {WorkflowId}", activity.GetType().Name, state.WorkflowId);
            throw new WorkflowExecutionException($"Activity {activity.GetType().Name} failed", ex);
        }
    }

    private async Task CompleteWorkflow(ImportWorkflowState state)
    {
        state.Status = WorkflowStatus.Completed;
        state.CompletionTime = DateTime.UtcNow;
        await _stateRepository.SaveStateAsync(state.WorkflowId, state);
        await _mediator.Publish(new WorkflowCompletedEvent(state.WorkflowId, state.CompletionTime.Value));
    }

    private async Task HandleWorkflowFailure(ImportWorkflowState state, Exception ex)
    {
        state.Status = WorkflowStatus.Failed;
        state.ErrorDetails = ex.ToString();
        state.CompletionTime = DateTime.UtcNow;
        
        await _stateRepository.SaveStateAsync(state.WorkflowId, state);
        await _mediator.Publish(new WorkflowFailedEvent(state.WorkflowId, ex));
        await CompensateFailedWorkflow(state);
    }

    private async Task CompensateFailedWorkflow(ImportWorkflowState state)
    {
        try
        {
            if (state.ProcessedFiles.Any())
            {
                await _importService.RollbackImportAsync(state.ProcessedFiles);
            }
            _logger.LogInformation("Compensation completed for workflow {WorkflowId}", state.WorkflowId);
        }
        catch (Exception compensationEx)
        {
            _logger.LogError(compensationEx, "Compensation failed for workflow {WorkflowId}", state.WorkflowId);
        }
    }
}