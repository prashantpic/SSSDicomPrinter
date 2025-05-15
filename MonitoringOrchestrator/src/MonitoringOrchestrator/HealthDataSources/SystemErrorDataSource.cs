using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TheSSS.DICOMViewer.Monitoring.Configuration;
using TheSSS.DICOMViewer.Monitoring.Contracts;
using TheSSS.DICOMViewer.Monitoring.Exceptions;
using TheSSS.DICOMViewer.Monitoring.Interfaces;
using TheSSS.DICOMViewer.Monitoring.Interfaces.Adapters; // Assuming ILoggerAdapter

namespace TheSSS.DICOMViewer.Monitoring.HealthDataSources;

public class SystemErrorDataSource : IHealthDataSource
{
    private readonly ISystemErrorLogAdapter _systemErrorLogAdapter;
    private readonly ILoggerAdapter<SystemErrorDataSource> _logger;
    private readonly MonitoringOptions _monitoringOptions; // To get lookback period

    public SystemErrorDataSource(ISystemErrorLogAdapter systemErrorLogAdapter, ILoggerAdapter<SystemErrorDataSource> logger, IOptions<MonitoringOptions> monitoringOptions)
    {
        _systemErrorLogAdapter = systemErrorLogAdapter;
        _logger = logger;
        _monitoringOptions = monitoringOptions.Value;
    }

    /// <inheritdoc/>
    public async Task<object> GetHealthDataAsync(CancellationToken cancellationToken)
    {
        _logger.Debug($"Collecting system error summary data (last {_monitoringOptions.ErrorLogLookbackPeriod}) via adapter.");
        try
        {
            var summary = await _systemErrorLogAdapter.GetCriticalErrorSummaryAsync(_monitoringOptions.ErrorLogLookbackPeriod, cancellationToken);
            _logger.Debug($"Successfully collected system error summary. Critical count: {summary.CriticalErrorCountLast24Hours}");
            return summary; // Return SystemErrorInfoSummaryDto
        }
        catch (DataSourceUnavailableException ex)
        {
            _logger.Error(ex, "System error log data source adapter reported unavailable.");
            throw; // Re-throw DataSourceUnavailableException
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "An unexpected error occurred while getting system error summary.");
            throw new DataSourceUnavailableException(nameof(SystemErrorDataSource), "Failed to retrieve system error summary due to an internal error.", ex);
        }
    }
}