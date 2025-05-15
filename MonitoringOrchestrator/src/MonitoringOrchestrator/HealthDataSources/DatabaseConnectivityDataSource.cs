using TheSSS.DICOMViewer.Monitoring.Interfaces;
using TheSSS.DICOMViewer.Monitoring.Interfaces.Adapters;
using TheSSS.DICOMViewer.Monitoring.Exceptions;
using Microsoft.Extensions.Logging;

namespace TheSSS.DICOMViewer.Monitoring.HealthDataSources;

public class DatabaseConnectivityDataSource : IHealthDataSource
{
    private readonly IDbConnectivityAdapter _adapter;
    private readonly ILogger<DatabaseConnectivityDataSource> _logger;
    public string Name => "Database";

    public DatabaseConnectivityDataSource(IDbConnectivityAdapter adapter, ILogger<DatabaseConnectivityDataSource> logger)
        => (_adapter, _logger) = (adapter, logger);

    public async Task<object> GetHealthDataAsync(CancellationToken cancellationToken)
    {
        try
        {
            return await _adapter.CheckDatabaseConnectivityAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database connectivity check failed");
            throw new DataSourceUnavailableException(Name, ex.Message, ex);
        }
    }
}