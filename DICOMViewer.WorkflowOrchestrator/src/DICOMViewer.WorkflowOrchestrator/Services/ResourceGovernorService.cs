using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace TheSSS.DICOMViewer.Application.WorkflowOrchestrator.Services;

public class ResourceGovernorService : IResourceGovernor
{
    private readonly WorkflowOrchestratorSettings _settings;
    private readonly ILogger<ResourceGovernorService> _logger;
    private readonly SemaphoreSlim _importThreadSemaphore;
    private readonly SemaphoreSlim _networkConnectionSemaphore;

    public ResourceGovernorService(
        IOptions<WorkflowOrchestratorSettings> settings,
        ILogger<ResourceGovernorService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
        _importThreadSemaphore = new SemaphoreSlim(_settings.MaxParallelFileProcessingThreadsPerImport);
        _networkConnectionSemaphore = new SemaphoreSlim(_settings.MaxConcurrentNetworkOperations);
    }

    public async Task<bool> TryAcquireResourceAsync(
        ResourceType type,
        int count,
        Guid workflowId,
        CancellationToken cancellationToken)
    {
        var semaphore = GetSemaphoreForType(type);
        var available = semaphore.CurrentCount >= count;

        if (available)
        {
            await semaphore.WaitAsync(cancellationToken);
            _logger.LogInformation(
                "Acquired {Count} resources of type {Type} for workflow {WorkflowId}",
                count,
                type,
                workflowId);
            return true;
        }

        _logger.LogWarning(
            "Resource constraint violated for {Type}. Available: {Available}, Requested: {Requested}",
            type,
            semaphore.CurrentCount,
            count);
        return false;
    }

    public void ReleaseResource(ResourceType type, int count, Guid workflowId)
    {
        var semaphore = GetSemaphoreForType(type);
        semaphore.Release(count);
        _logger.LogInformation(
            "Released {Count} resources of type {Type} for workflow {WorkflowId}",
            count,
            type,
            workflowId);
    }

    private SemaphoreSlim GetSemaphoreForType(ResourceType type)
    {
        return type switch
        {
            ResourceType.ImportThread => _importThreadSemaphore,
            ResourceType.NetworkConnection => _networkConnectionSemaphore,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type,