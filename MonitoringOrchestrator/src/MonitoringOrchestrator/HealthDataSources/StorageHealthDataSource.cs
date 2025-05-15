using Microsoft.Extensions.Logging;
using TheSSS.DICOMViewer.Monitoring.Interfaces;
using TheSSS.DICOMViewer.Monitoring.Interfaces.Adapters;
using TheSSS.DICOMViewer.Monitoring.Exceptions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace TheSSS.DICOMViewer.Monitoring.HealthDataSources
{
    /// <summary>
    /// Implementation of <see cref="IHealthDataSource"/> for monitoring storage health.
    /// Provides health information related to local DICOM repository storage utilization.
    /// </summary>
    public class StorageHealthDataSource : IHealthDataSource
    {
        private readonly IStorageInfoAdapter _storageInfoAdapter;
        private readonly ILogger<StorageHealthDataSource> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="StorageHealthDataSource"/> class.
        /// </summary>
        /// <param name="storageInfoAdapter">The adapter for retrieving storage information.</param>
        /// <param name="logger">The logger.</param>
        public StorageHealthDataSource(
            IStorageInfoAdapter storageInfoAdapter,
            ILogger<StorageHealthDataSource> logger)
        {
            _storageInfoAdapter = storageInfoAdapter ?? throw new ArgumentNullException(nameof(storageInfoAdapter));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc/>
        public async Task<object> GetHealthDataAsync(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogDebug("Fetching storage health information.");
                var storageHealthInfo = await _storageInfoAdapter.GetStorageHealthInfoAsync(cancellationToken);
                _logger.LogDebug("Successfully fetched storage health information.");
                return storageHealthInfo;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve storage health information.");
                throw new DataSourceUnavailableException("Failed to retrieve storage health information.", ex, nameof(StorageHealthDataSource));
            }
        }
    }
}