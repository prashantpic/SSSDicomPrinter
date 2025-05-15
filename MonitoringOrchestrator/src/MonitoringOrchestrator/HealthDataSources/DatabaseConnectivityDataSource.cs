using TheSSS.DICOMViewer.Monitoring.Interfaces;
using TheSSS.DICOMViewer.Monitoring.Interfaces.Adapters;
using TheSSS.DICOMViewer.Monitoring.Exceptions;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace TheSSS.DICOMViewer.Monitoring.HealthDataSources
{
    public class DatabaseConnectivityDataSource : IHealthDataSource
    {
        private readonly IDbConnectivityAdapter _dbConnectivityAdapter;
        private readonly ILogger<DatabaseConnectivityDataSource> _logger;

        public string Name => "Database";

        public DatabaseConnectivityDataSource(
            IDbConnectivityAdapter dbConnectivityAdapter,
            ILogger<DatabaseConnectivityDataSource> logger)
        {
            _dbConnectivityAdapter = dbConnectivityAdapter ?? throw new ArgumentNullException(nameof(dbConnectivityAdapter));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<object> GetHealthDataAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("Attempting to check database connectivity for data source: {DataSourceName}.", Name);
            try
            {
                var dbInfo = await _dbConnectivityAdapter.CheckDatabaseConnectivityAsync(cancellationToken);
                _logger.LogInformation("Database connectivity check completed for data source: {DataSourceName}. Connected: {IsConnected}.", Name, dbInfo.IsConnected);
                return dbInfo;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking database connectivity for data source: {DataSourceName}.", Name);
                throw new DataSourceUnavailableException(Name, $"Failed to check database connectivity due to: {ex.Message}", ex);
            }
        }
    }
}