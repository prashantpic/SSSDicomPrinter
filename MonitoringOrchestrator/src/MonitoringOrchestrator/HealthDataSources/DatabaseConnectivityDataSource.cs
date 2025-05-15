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
    /// Implementation of <see cref="IHealthDataSource"/> for monitoring database connectivity.
    /// Provides health information related to database connectivity.
    /// </summary>
    public class DatabaseConnectivityDataSource : IHealthDataSource
    {
        private readonly IDbConnectivityAdapter _dbConnectivityAdapter;
        private readonly ILogger<DatabaseConnectivityDataSource> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseConnectivityDataSource"/> class.
        /// </summary>
        /// <param name="dbConnectivityAdapter">The adapter for checking database connectivity.</param>
        /// <param name="logger">The logger.</param>
        public DatabaseConnectivityDataSource(
            IDbConnectivityAdapter dbConnectivityAdapter,
            ILogger<DatabaseConnectivityDataSource> logger)
        {
            _dbConnectivityAdapter = dbConnectivityAdapter ?? throw new ArgumentNullException(nameof(dbConnectivityAdapter));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc/>
        public async Task<object> GetHealthDataAsync(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogDebug("Checking database connectivity.");
                var dbConnectivityInfo = await _dbConnectivityAdapter.CheckDatabaseConnectivityAsync(cancellationToken);
                _logger.LogDebug("Successfully checked database connectivity. Status: {IsConnected}", dbConnectivityInfo.IsConnected);
                return dbConnectivityInfo;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to check database connectivity.");
                throw new DataSourceUnavailableException("Failed to check database connectivity.", ex, nameof(DatabaseConnectivityDataSource));
            }
        }
    }
}