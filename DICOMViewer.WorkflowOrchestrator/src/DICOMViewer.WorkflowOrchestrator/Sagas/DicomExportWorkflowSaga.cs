using MediatR;
using TheSSS.DICOMViewer.Application.WorkflowOrchestrator.Interfaces;

namespace TheSSS.DICOMViewer.Application.WorkflowOrchestrator.Sagas;

public class DicomExportWorkflowSaga
{
    private readonly IWorkflowStateRepository _stateRepository;
    private readonly IDicomNetworkServiceAdapter _networkService;
    private readonly IMediator _mediator;

    public DicomExportWorkflowSaga(
        IWorkflowStateRepository stateRepository,
        IDicomNetworkServiceAdapter networkService,
        IMediator mediator)
    {
        _stateRepository = stateRepository;
        _networkService = networkService;
        _mediator = mediator;
    }

    public async Task HandleStartCommand(StartDicomExportWorkflowCommand command)
    {
        var state = new ExportWorkflowState
        {
            WorkflowId = command.WorkflowId,
            StudyId = command.StudyId,
            DestinationPath = command.DestinationPath,
            Status = "Initializing"
        };

        await _stateRepository.SaveStateAsync(state);
        await ExportData(state);
    }

    private async Task ExportData(ExportWorkflowState state)
    {
        state.Status = "Exporting";
        await _stateRepository.SaveStateAsync(state);

        try
        {
            // Implementation would include actual export logic
            await _networkService.SendCStoreAsync(
                Guid.Empty, // PACS node ID placeholder
                new List<string>(), // File paths placeholder
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
                "Export"));
            throw;
        }
        finally
        {
            await _stateRepository.SaveStateAsync(state);
        }
    }
}