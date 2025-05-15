namespace TheSSS.DICOMViewer.Monitoring.UseCaseHandlers;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TheSSS.DICOMViewer.Monitoring.Configuration;
using System;
using System.Threading;
using System.Threading.Tasks;
using TheSSS.DICOMViewer.Monitoring.Integrations; // For PrometheusMetricsCollector

public class SystemHealthMonitorWorker : BackgroundService
{
    private readonly ILogger<SystemHealthMonitorWorker> _logger;
    private readonly MonitoringOptions _monitoringOptions;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public SystemHealthMonitorWorker(
        ILogger<SystemHealthMonitorWorker> logger,
        IOptions<MonitoringOptions> monitoringOptions,
        IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _monitoringOptions = monitoringOptions.Value;
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("System Health Monitor Worker started.");

        if (!_monitoringOptions.IsMonitoringEnabled)
        {
            _logger.LogInformation("System Monitoring is disabled by configuration. Worker will not run periodic checks.");
            return;
        }

        var interval = _monitoringOptions.SystemHealthCheckInterval;
        if (interval <= TimeSpan.Zero)
        {
             _logger.LogError($"SystemHealthCheckInterval is configured incorrectly ({interval}). Monitoring worker cannot start.");
             // Optionally, throw an exception here to stop the host application if this is critical
             // throw new ArgumentOutOfRangeException(nameof(_monitoringOptions.SystemHealthCheckInterval), "SystemHealthCheckInterval must be positive.");
             return;
        }


        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation($"System Health Monitor Worker running at: {DateTimeOffset.Now}");

            try
            {
                // Use a service scope to resolve services for each iteration
                // This is important if HealthAggregationService or AlertEvaluationService are scoped
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var healthAggregationService = scope.ServiceProvider.GetRequiredService<HealthAggregationService>();
                    var alertEvaluationService = scope.ServiceProvider.GetRequiredService<AlertEvaluationService>();
                    var prometheusCollector = scope.ServiceProvider.GetService<PrometheusMetricsCollector>(); // Optional

                    _logger.LogDebug("Beginning health data aggregation.");
                    var healthReport = await healthAggregationService.AggregateHealthDataAsync(stoppingToken);
                    _logger.LogDebug($"Health data aggregation completed. Overall status: {healthReport.OverallStatus}");

                    _logger.LogDebug("Beginning alert evaluation.");
                    await alertEvaluationService.EvaluateHealthReportAsync(healthReport, stoppingToken);
                    _logger.LogDebug("Alert evaluation completed.");

                    if (prometheusCollector != null)
                    {
                        _logger.LogDebug("Updating Prometheus metrics.");
                        prometheusCollector.UpdateMetrics(healthReport);
                        _logger.LogDebug("Prometheus metrics update completed.");
                    }
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("System Health Monitor Worker stopping due to cancellation request.");
                break; // Exit the loop gracefully
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled error occurred during the health monitoring cycle.");
                // Depending on severity, might want to trigger an alert here for the monitoring system itself
                // Or simply log and continue. If this loop fails consistently, the system won't be monitored.
            }

            // Wait for the next interval
            _logger.LogInformation($"System Health Monitor Worker finished cycle. Waiting for {interval}...");
            try
            {
                await Task.Delay(interval, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                 _logger.LogInformation("System Health Monitor Worker stopping during delay due to cancellation request.");
                break; // Exit the loop gracefully
            }
        }

        _logger.LogInformation("System Health Monitor Worker stopped.");
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("System Health Monitor Worker is stopping.");
        await base.StopAsync(cancellationToken);
    }
}