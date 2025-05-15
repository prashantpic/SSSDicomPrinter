using TheSSS.DICOMViewer.Monitoring.Interfaces;
using TheSSS.DICOMViewer.Monitoring.Interfaces.Adapters;
using TheSSS.DICOMViewer.Monitoring.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace TheSSS.DICOMViewer.Monitoring.HealthDataSources;

public class SystemErrorDataSource : IHealthDataSource
{
    private readonly ISystemErrorLogAdapter _adapter;
    private readonly MonitoringOptions _options;
    private readonly ILogger<SystemErrorDataSource> _logger;
    public string Name => "SystemErrors";

    public SystemErrorDataSource(
        ISystemErrorLogAdapter adapter,
        IOptions<MonitoringOptions> options,
        ILogger<SystemErrorDataSource> logger)
        => (_adapter, _options, _logger) = (adapter, options.Value, logger);

    public async Task<object> GetHealthDataAsync(CancellationToken cancellationToken)
    {
        try
        {
            return await _adapter.GetCriticalErrorSummaryAsync(_options.CriticalErrorLookbackPeriod, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "System error check failed");
            throw new DataSourceUnavailableException(Name, ex.Message, ex);
        }
    }
}