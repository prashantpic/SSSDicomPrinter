using TheSSS.DICOMViewer.Monitoring.Interfaces;
using TheSSS.DICOMViewer.Monitoring.Interfaces.Adapters;
using TheSSS.DICOMViewer.Monitoring.Exceptions;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace TheSSS.DICOMViewer.Monitoring.HealthDataSources
{
    public class StorageHealthDataSource : IHealthDataSource
    {
        private readonly IStorageInfoAdapter _storageInfoAdapter;
        private readonly ILogger<StorageHealthDataSource> _logger;

        public string Name => "Storage";

        public StorageHealthDataSource(
            IStorageInfoAdapter storageInfoAdapter,
            ILogger<StorageHealthDataSource> logger)
        {
            _storageInfoAdapter = storageInfoAdapter ?? throw new ArgumentNullException(nameof(storageInfoAdapter));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<object> GetHealthDataAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("Attempting to retrieve storage health information for data source: {DataSourceName}.", Name);
            try
            {
                var storageInfo = await _storageInfoAdapter.GetStorageHealthInfoAsync(cancellationToken);
                _logger.LogInformation("Successfully retrieved storage health information for data source: {DataSourceName}. Used: {UsedPercentage}%.", Name, storageInfo.UsedPercentage);
                return storageInfo;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving storage health information for data source: {DataSourceName}.", Name);
                throw new DataSourceUnavailableException(Name, $"Failed to retrieve storage health information due to: {ex.Message}", ex);
            }
        }
    }
}