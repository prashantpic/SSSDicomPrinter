using Microsoft.Extensions.Logging;
using TheSSS.DICOMViewer.Application.WorkflowOrchestrator.Interfaces;
using TheSSS.DICOMViewer.Application.WorkflowOrchestrator.Policies;

namespace TheSSS.DICOMViewer.Application.WorkflowOrchestrator.Services;

public class NetworkOperationCoordinator
{
    private readonly IDicomNetworkServiceAdapter _networkService;
    private readonly NetworkRetryPolicyProvider _policyProvider;
    private readonly IAuditLoggerAdapter _auditLogger;
    private readonly ILogger<NetworkOperationCoordinator> _logger;

    public NetworkOperationCoordinator(
        IDicomNetworkServiceAdapter networkService,
        NetworkRetryPolicyProvider policyProvider,
        IAuditLoggerAdapter auditLogger,
        ILogger<NetworkOperationCoordinator> logger)
    {
        _networkService = networkService;
        _policyProvider = policyProvider;
        _auditLogger = auditLogger;
        _logger = logger;
    }

    public async Task PerformCStoreAsync(
        Guid pacsNodeId,
        IEnumerable<string> dicomFiles,
        Guid workflowId,
        CancellationToken cancellationToken)
    {
        var policy = _policyProvider.GetNetworkPolicy();
        
        await policy.ExecuteAsync(async () =>
        {
            try
            {
                await _networkService.SendCStoreAsync(
                    pacsNodeId,
                    dicomFiles,
                    workflowId,
                    cancellationToken);
                
                await _auditLogger.LogAuditEventAsync(
                    "CStoreSuccess",
                    $"Successfully stored {dicomFiles.Count()} instances",
                    null,
                    null,
                    null,
                    workflowId,
                    null);
            }
            catch (Exception ex)
            {
                await _auditLogger.LogAuditEventAsync(
                    "CStoreFailure",
                    $"Failed to store instances: {ex.Message}",
                    null,
                    null,
                    null,
                    workflowId,
                    null);
                throw;
            }
        });
    }

    public async Task<List<QueryResult>> PerformCFindAsync(
        Guid pacsNodeId,
        QueryParameters query,
        Guid workflowId,
        CancellationToken cancellationToken)
    {
        var policy = _policyProvider.GetNetworkPolicy();
        return await policy.ExecuteAsync(async () =>
        {
            return await _networkService.SendCFindAsync(
                pacsNodeId,
                query,
                workflowId,
                cancellationToken);
        });
    }
}