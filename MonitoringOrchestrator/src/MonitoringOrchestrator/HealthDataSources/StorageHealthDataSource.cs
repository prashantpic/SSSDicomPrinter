using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TheSSS.DICOMViewer.Monitoring.Contracts;
using TheSSS.DICOMViewer.Monitoring.Exceptions;
using TheSSS.DICOMViewer.Monitoring.Interfaces;
using TheSSS.DICOMViewer.Monitoring.Interfaces.Adapters; // Assuming ILoggerAdapter

namespace TheSSS.DICOMViewer.Monitoring.HealthDataSources;

public class StorageHealthDataSource : IHealthDataSource
{
    private readonly IStorageInfoAdapter _storageInfoAdapter;
    private readonly ILoggerAdapter<StorageHealthDataSource> _logger;

    public StorageHealthDataSource(IStorageInfoAdapter storageInfoAdapter, ILoggerAdapter<StorageHealthDataSource> logger)
    {
        _storageInfoAdapter = storageInfoAdapter;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<object> GetHealthDataAsync(CancellationToken cancellationToken)
    {
        _logger.Debug("Collecting storage health data via adapter.");
        try
        {
            var info = await _storageInfoAdapter.GetStorageHealthInfoAsync(cancellationToken);
            _logger.Debug("Successfully collected storage health data.");
            return info;
        }
        catch (DataSourceUnavailableException ex)
        {
            _logger.Error(ex, "Storage data source adapter reported unavailable.");
            throw; // Re-throw DataSourceUnavailableException
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "An unexpected error occurred while getting storage health data.");
            // Wrap unexpected exceptions in DataSourceUnavailableException
            throw new DataSourceUnavailableException(nameof(StorageHealthDataSource), "Failed to retrieve storage health data due to an internal error.", ex);
        }
    }
}