using TheSSS.DICOMViewer.Monitoring.Interfaces;
using TheSSS.DICOMViewer.Monitoring.Interfaces.Adapters;
using TheSSS.DICOMViewer.Monitoring.Exceptions;
using Microsoft.Extensions.Logging;

namespace TheSSS.DICOMViewer.Monitoring.HealthDataSources;

public class StorageHealthDataSource : IHealthDataSource
{
    private readonly IStorageInfoAdapter _adapter;
    private readonly ILogger<StorageHealthDataSource> _logger;
    public string Name => "Storage";

    public StorageHealthDataSource(IStorageInfoAdapter adapter, ILogger<StorageHealthDataSource> logger)
        => (_adapter, _logger) = (adapter, logger);

    public async Task<object> GetHealthDataAsync(CancellationToken cancellationToken)
    {
        try
        {
            return await _adapter.GetStorageHealthInfoAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Storage health check failed");
            throw new DataSourceUnavailableException(Name, ex.Message, ex);
        }
    }
}