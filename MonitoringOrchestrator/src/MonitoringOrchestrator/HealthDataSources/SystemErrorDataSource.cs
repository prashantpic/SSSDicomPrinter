using TheSSS.DICOMViewer.Monitoring.Interfaces;
using TheSSS.DICOMViewer.Monitoring.Interfaces.Adapters;
using TheSSS.DICOMViewer.Monitoring.Configuration;
using TheSSS.DICOMViewer.Monitoring.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace TheSSS.DICOMViewer.Monitoring.HealthDataSources
{
    public class SystemErrorDataSource : IHealthDataSource
    {
        private readonly ISystemErrorLogAdapter _systemErrorLogAdapter;
        private readonly MonitoringOptions _monitoringOptions;
        private readonly ILogger<SystemErrorDataSource> _logger;

        public string Name => "SystemErrors";

        public SystemErrorDataSource(
            ISystemErrorLogAdapter systemErrorLogAdapter,
            IOptions<MonitoringOptions> monitoringOptions,
            ILogger<SystemErrorDataSource> logger)
        {
            _systemErrorLogAdapter = systemErrorLogAdapter ?? throw new ArgumentNullException(nameof(systemErrorLogAdapter));
            _monitoringOptions = monitoringOptions?.Value ?? throw new ArgumentNullException(nameof(monitoringOptions));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<object> GetHealthDataAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("Attempting to retrieve system error summary for data source: {DataSourceName} with lookback period: {LookbackPeriod}.", Name, _monitoringOptions.CriticalErrorLookbackPeriod);
            try
            {
                var errorSummary = await _systemErrorLogAdapter.GetCriticalErrorSummaryAsync(_monitoringOptions.CriticalErrorLookbackPeriod, cancellationToken);
                _logger.LogInformation("Successfully retrieved system error summary for data source: {DataSourceName}. CriticalErrorCount: {Count}.", Name, errorSummary.CriticalErrorCountLast24Hours);
                return errorSummary;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving system error summary for data source: {DataSourceName}.", Name);
                throw new DataSourceUnavailableException(Name, $"Failed to retrieve system error summary due to: {ex.Message}", ex);
            }
        }
    }
}