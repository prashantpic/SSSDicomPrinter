using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TheSSS.DICOMViewer.Monitoring.Contracts;
using TheSSS.DICOMViewer.Monitoring.Exceptions;
using TheSSS.DICOMViewer.Monitoring.Interfaces;
using TheSSS.DICOMViewer.Monitoring.Interfaces.Adapters; // Assuming ILoggerAdapter

namespace TheSSS.DICOMViewer.Monitoring.HealthDataSources;

public class DatabaseConnectivityDataSource : IHealthDataSource
{
    private readonly IDbConnectivityAdapter _dbConnectivityAdapter;
    private readonly ILoggerAdapter<DatabaseConnectivityDataSource> _logger;


    public DatabaseConnectivityDataSource(IDbConnectivityAdapter dbConnectivityAdapter, ILoggerAdapter<DatabaseConnectivityDataSource> logger)
    {
        _dbConnectivityAdapter = dbConnectivityAdapter;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<object> GetHealthDataAsync(CancellationToken cancellationToken)
    {
        _logger.Debug("Checking database connectivity via adapter.");
        try
        {
            var info = await _dbConnectivityAdapter.CheckDatabaseConnectivityAsync(cancellationToken);
            _logger.Debug($"Database connectivity check completed. Connected: {info.IsConnected}");
            return info;
        }
        catch (DataSourceUnavailableException ex)
        {
            _logger.Error(ex, "Database connectivity data source adapter reported unavailable.");
            throw; // Re-throw DataSourceUnavailableException
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "An unexpected error occurred while checking database connectivity.");
            throw new DataSourceUnavailableException(nameof(DatabaseConnectivityDataSource), "Failed to check database connectivity due to an internal error.", ex);
        }
    }
}