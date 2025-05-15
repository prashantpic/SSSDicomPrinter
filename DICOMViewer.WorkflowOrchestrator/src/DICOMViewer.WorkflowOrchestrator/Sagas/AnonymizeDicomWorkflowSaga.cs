using MediatR;
using TheSSS.DICOMViewer.Application.WorkflowOrchestrator.Interfaces;

namespace TheSSS.DICOMViewer.Application.WorkflowOrchestrator.Sagas;

public class AnonymizeDicomWorkflowSaga
{
    private readonly IWorkflowStateRepository _stateRepository;
    private readonly IAnonymizationServiceAdapter _anonymizationService;
    private readonly IAuditLoggerAdapter _auditLogger;
    private readonly IMediator _mediator;

    public AnonymizeDicomWorkflowSaga(
        IWorkflowStateRepository stateRepository,
        IAnonymizationServiceAdapter anonymizationService,
        IAuditLoggerAdapter auditLogger,
        IMediator mediator)
    {
        _stateRepository = stateRepository;
        _anonymizationService = anonymizationService;
        _auditLogger = auditLogger;
        _mediator = mediator;
    }

    public async Task HandleStartCommand(StartDicomAnonymizationWorkflowCommand command)
    {
        var state = new AnonymizationWorkflowState
        {
            WorkflowId = command.WorkflowId,
            StudyId = command.StudyId,
            AnonymizationProfileId = command.AnonymizationProfileId,
            Status = "Initializing"
        };

        await _stateRepository.SaveStateAsync(state);
        await AnonymizeStudy(state);
    }

    private async Task AnonymizeStudy(AnonymizationWorkflowState state)
    {
        state.Status = "Processing";
        await _stateRepository.SaveStateAsync(state);

        try
        {
            var result = await _anonymizationService.AnonymizeStudyAsync(
                state.StudyId,
                state.AnonymizationProfileId,
                state.WorkflowId,
                CancellationToken.None);

            if (result.Success)
            {
                await LogSuccess(state);
                await _mediator.Publish(new WorkflowCompletedEvent(state.WorkflowId, DateTime.UtcNow));
            }
            else
            {
                throw new WorkflowExecutionException($"Anonymization failed: {result.ErrorMessage}");
            }
        }
        catch (Exception ex)
        {
            await HandleFailure(state, ex);
            throw;
        }
    }

    private async Task LogSuccess(AnonymizationWorkflowState state)
    {
        await _auditLogger.LogAuditEventAsync(
            "AnonymizationComplete",
            $"Study {state.StudyId} anonymized with profile {state.AnonymizationProfileId}",
            null,
            state.StudyId,
            null,
            state.WorkflowId,
            null);
    }

    private async Task HandleFailure(AnonymizationWorkflowState state, Exception ex)
    {
        state.Status = "Failed";
        await _stateRepository.SaveStateAsync(state);
        
        await _mediator.Publish(new WorkflowFailedEvent(
            state.WorkflowId,
            DateTime.UtcNow,
            ex.Message,
            ex.ToString(),
            "Anonymization"));
    }
}