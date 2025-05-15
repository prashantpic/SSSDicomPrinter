using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TheSSS.DICOMViewer.Monitoring.Configuration;
using TheSSS.DICOMViewer.Monitoring.Contracts; // Assuming HealthReportDto is here
// Assuming AlertEvaluationService and HealthAggregationService interfaces/classes are defined elsewhere
// and will be available through DI. For now, we'll assume their existence.

namespace TheSSS.DICOMViewer.Monitoring.UseCaseHandlers
{
    /// <summary>
    /// Background worker service that periodically triggers health data collection,
    /// aggregation, and alert evaluation.
    /// It drives the continuous monitoring cycle of the system's health.
    /// </summary>
    public class SystemHealthMonitorWorker : BackgroundService
    {
        private readonly ILogger<SystemHealthMonitorWorker> _logger;
        private readonly MonitoringOptions _monitoringOptions;
        private readonly HealthAggregationService _healthAggregationService; // Assuming this concrete type or an interface
        private readonly AlertEvaluationService _alertEvaluationService; // Assuming this concrete type or an interface

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemHealthMonitorWorker"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="monitoringOptions">Monitoring configuration options.</param>
        /// <param name="healthAggregationService">Service for aggregating health data.</param>
        /// <param name="alertEvaluationService">Service for evaluating health reports and triggering alerts.</param>
        public SystemHealthMonitorWorker(
            ILogger<SystemHealthMonitorWorker> logger,
            IOptions<MonitoringOptions> monitoringOptions,
            HealthAggregationService healthAggregationService, // To be replaced by IHealthAggregationService if defined
            AlertEvaluationService alertEvaluationService)    // To be replaced by IAlertEvaluationService if defined
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _monitoringOptions = monitoringOptions?.Value ?? throw new ArgumentNullException(nameof(monitoringOptions));
            _healthAggregationService = healthAggregationService ?? throw new ArgumentNullException(nameof(healthAggregationService));
            _alertEvaluationService = alertEvaluationService ?? throw new ArgumentNullException(nameof(alertEvaluationService));

            if (_monitoringOptions.SystemHealthCheckInterval <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(monitoringOptions), "SystemHealthCheckInterval must be a positive TimeSpan.");
            }
        }

        /// <summary>
        /// This method is called when the <see cref="IHostedService"/> starts.
        /// The implementation should return a task that represents the lifetime of the long
        /// running operation(s) being performed.
        /// </summary>
        /// <param name="stoppingToken">Triggered when <see cref="IHostedService.StopAsync(CancellationToken)"/> is called.</param>
        /// <returns>A <see cref="Task"/> that represents the long running operations.</returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("SystemHealthMonitorWorker started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if (!_monitoringOptions.IsMonitoringEnabled)
                    {
                        _logger.LogTrace("System monitoring is disabled. Skipping health check cycle.");
                        await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken); // Check periodically if it's re-enabled
                        continue;
                    }

                    _logger.LogInformation("Starting system health check cycle at {Timestamp}.", DateTimeOffset.UtcNow);

                    HealthReportDto healthReport = await _healthAggregationService.AggregateHealthAsync(stoppingToken);
                    _logger.LogInformation("Health aggregation completed. Overall system status: {SystemStatus}", healthReport.SystemStatus);

                    // Pass the aggregated health report to the alert evaluation service
                    await _alertEvaluationService.EvaluateHealthReportAsync(healthReport, stoppingToken);

                    _logger.LogInformation("System health check cycle completed at {Timestamp}.", DateTimeOffset.UtcNow);
                }
                catch (OperationCanceledException)
                {
                    // This is expected if stoppingToken is cancelled during an operation or delay.
                    _logger.LogInformation("SystemHealthMonitorWorker stopping due to cancellation request.");
                    break; // Exit the loop if cancellation is requested
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An unexpected error occurred during the health check cycle.");
                    // Depending on the severity, we might want to alert on this critical failure separately
                    // or implement a more resilient retry mechanism for the worker itself.
                }

                try
                {
                    // Wait for the configured interval before the next cycle
                    if (!stoppingToken.IsCancellationRequested)
                    {
                         _logger.LogTrace("Waiting for {Interval} before next health check cycle.", _monitoringOptions.SystemHealthCheckInterval);
                        await Task.Delay(_monitoringOptions.SystemHealthCheckInterval, stoppingToken);
                    }
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("SystemHealthMonitorWorker delay was canceled. Worker is stopping.");
                    break; // Exit the loop if cancellation is requested during delay
                }
            }

            _logger.LogInformation("SystemHealthMonitorWorker stopped.");
        }
    }
}