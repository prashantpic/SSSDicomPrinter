using MediatR;
using TheSSS.DICOMViewer.Application.WorkflowOrchestrator.Activities;
using TheSSS.DICOMViewer.Application.WorkflowOrchestrator.Interfaces;
using TheSSS.DICOMViewer.Application.WorkflowOrchestrator.Sagas.State;

namespace TheSSS.DICOMViewer.Application.WorkflowOrchestrator.Sagas;

public class ImportDicomWorkflowSaga
{
    private readonly IWorkflowStateRepository _stateRepository;
    private readonly IResourceGovernor _resourceGovernor;
    private readonly IWorkflowProgressReporter _progressReporter;
    private readonly IDicomImportServiceAdapter _importService;
    private readonly IMediator _mediator;
    
    public ImportDicomWorkflowSaga(
        IWorkflowStateRepository stateRepository,
        IResourceGovernor resourceGovernor,
        IWorkflowProgressReporter progressReporter,
        IDicomImportServiceAdapter importService,
        IMediator mediator)
    {
        _stateRepository = stateRepository;
        _resourceGovernor = resourceGovernor;
        _progressReporter = progressReporter;
        _importService = importService;
        _mediator = mediator;
    }

    public async Task HandleStartCommand(StartDicomImportWorkflowCommand command)
    {
        var state = new ImportWorkflowState
        {
            WorkflowId = command.WorkflowId,
            SourcePath = command.SourcePath,
            FilesToProcess = await DiscoverFiles(command),
            Options = command.Options,
            Status = "Initializing"
        };

        await _stateRepository.SaveStateAsync(state);
        await ProcessFiles(state);
    }

    private async Task ProcessFiles(ImportWorkflowState state)
    {
        state.Status = "Processing";
        await _stateRepository.SaveStateAsync(state);
        
        var parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = await GetMaxParallelism()
        };

        await Parallel.ForEachAsync(state.FilesToProcess, parallelOptions, async (file, ct) =>
        {
            try
            {
                await ProcessSingleFile(state, file);
            }
            catch (Exception ex)
            {
                HandleFileError(state, file, ex);
            }
        });

        state.Status = state.FailedFiles.Count > 0 ? "CompletedWithErrors" : "Completed";
        await _stateRepository.SaveStateAsync(state);
        await _mediator.Publish(new WorkflowCompletedEvent(state.WorkflowId, DateTime.UtcNow));
    }

    private async Task ProcessSingleFile(ImportWorkflowState state, string filePath)
    {
        var validationResult = await _importService.ValidateFileAsync(filePath, state.WorkflowId, CancellationToken.None);
        if (!validationResult.IsValid)
        {
            throw new WorkflowExecutionException($"Invalid DICOM file: {filePath}");
        }

        var importResult = await _importService.ImportFileAsync(filePath, state.WorkflowId, CancellationToken.None);
        state.ProcessedFiles.Add(filePath);
        await UpdateProgress(state);
    }

    private async Task UpdateProgress(ImportWorkflowState state)
    {
        state.ProcessedFilesCount = state.ProcessedFiles.Count;
        await _progressReporter.ReportProgressAsync(
            state.WorkflowId,
            (int)((double)state.ProcessedFilesCount / state.TotalFilesCount * 100),
            $"Processing {state.ProcessedFilesCount}/{state.TotalFilesCount} files",
            "FileImport");
    }

    private void HandleFileError(ImportWorkflowState state, string filePath, Exception ex)
    {
        state.FailedFiles.Add(filePath);
        _mediator.Publish(new WorkflowFailedEvent(
            state.WorkflowId,
            DateTime.UtcNow,
            ex.Message,
            ex.ToString(),
            "FileProcessing"));
    }

    private async Task<int> GetMaxParallelism()
    {
        return await _resourceGovernor.TryAcquireResourceAsync(
            ResourceType.ImportThread, 
            Environment.ProcessorCount, 
            Guid.Empty, 
            CancellationToken.None)
            ? Environment.ProcessorCount
            : 1;
    }

    private async Task<List<string>> DiscoverFiles(StartDicomImportWorkflowCommand command)
    {
        // Implementation omitted for brevity
        return new List<string>();
    }
}