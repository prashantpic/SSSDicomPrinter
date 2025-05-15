using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TheSSS.DICOMViewer.Monitoring.Configuration;
using TheSSS.DICOMViewer.Monitoring.Interfaces;
using TheSSS.DICOMViewer.Monitoring.Interfaces.Adapters;
using TheSSS.DICOMViewer.Monitoring.Exceptions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace TheSSS.DICOMViewer.Monitoring.HealthDataSources
{
    /// <summary>
    /// Implementation of <see cref="IHealthDataSource"/> for monitoring critical system errors.
    /// Provides summaries of recent critical system errors logged by the application.
    /// </summary>
    public class SystemErrorDataSource : IHealthDataSource
    {
        private readonly ISystemErrorLogAdapter _systemErrorLogAdapter;
        private readonly IOptions<MonitoringOptions> _monitoringOptions;
        private readonly ILogger<SystemErrorDataSource> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemErrorDataSource"/> class.
        /// </summary>
        /// <param name="systemErrorLogAdapter">The adapter for retrieving system error summaries.</param>
        /// <param name="monitoringOptions">The monitoring configuration options.</param>
        /// <param name="logger">The logger.</param>
        public SystemErrorDataSource(
            ISystemErrorLogAdapter systemErrorLogAdapter,
            IOptions<MonitoringOptions> monitoringOptions,
            ILogger<SystemErrorDataSource> logger)
        {
            _systemErrorLogAdapter = systemErrorLogAdapter ?? throw new ArgumentNullException(nameof(systemErrorLogAdapter));
            _monitoringOptions = monitoringOptions ?? throw new ArgumentNullException(nameof(monitoringOptions));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc/>
        public async Task<object> GetHealthDataAsync(CancellationToken cancellationToken)
        {
            try
            {
                var lookbackWindow = _monitoringOptions.Value.SystemErrorLookbackWindow;
                _logger.LogDebug("Fetching system error summary for the last {LookbackWindow}.", lookbackWindow);
                var errorSummary = await _systemErrorLogAdapter.GetCriticalErrorSummaryAsync(lookbackWindow, cancellationToken);
                _logger.LogDebug("Successfully fetched system error summary. Critical errors: {CriticalCount}", errorSummary.CriticalErrorCountLast24Hours);
                return errorSummary;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve system error summary.");
                throw new DataSourceUnavailableException("Failed to retrieve system error summary.", ex, nameof(SystemErrorDataSource));
            }
        }
    }
}