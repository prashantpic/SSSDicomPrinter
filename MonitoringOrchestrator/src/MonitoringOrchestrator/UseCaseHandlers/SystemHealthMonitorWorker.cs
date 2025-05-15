using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using TheSSS.DICOMViewer.Monitoring.Configuration;
using TheSSS.DICOMViewer.Monitoring.Interfaces.Adapters;

namespace TheSSS.DICOMViewer.Monitoring.UseCaseHandlers;

public class SystemHealthMonitorWorker : BackgroundService
{
    private readonly ILoggerAdapter<SystemHealthMonitorWorker> _logger;
    private readonly MonitoringOptions _monitoringOptions;
    private readonly HealthAggregationService _healthAggregationService;
    private readonly AlertEvaluationService _alertEvaluationService;
    private readonly IAuditLoggingAdapter _auditLoggingAdapter;

    public SystemHealthMonitorWorker(
        ILoggerAdapter<SystemHealthMonitorWorker> logger,
        IOptions<MonitoringOptions> monitoringOptions,
        HealthAggregationService healthAggregationService,
        AlertEvaluationService alertEvaluationService,
        IAuditLoggingAdapter auditLoggingAdapter)
    {
        _logger = logger;
        _monitoringOptions = monitoringOptions.Value;
        _healthAggregationService = healthAggregationService;
        _alertEvaluationService = alertEvaluationService;
        _auditLoggingAdapter = auditLoggingAdapter;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.Info("SystemHealthMonitorWorker started.");
        await _auditLoggingAdapter.LogAuditEventAsync(
            eventType: "MonitoringWorker",
            eventDetails: "SystemHealthMonitorWorker starting.",
            outcome: "Success",
            sourceComponent: nameof(SystemHealthMonitorWorker));

        // Delay startup slightly to allow other services to initialize, if necessary
        // This can be configurable or removed if not needed.
        try
        {
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken); 
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
             _logger.Info("SystemHealthMonitorWorker stopping during initial delay.");
            await LogStopAsync();
            return;
        }


        while (!stoppingToken.IsCancellationRequested)
        {
            if (!_monitoringOptions.IsMonitoringEnabled)
            {
                _logger.Info("System monitoring is disabled. Worker is sleeping.");
                try
                {
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken); // Sleep longer if disabled
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break; // Exit loop if cancellation requested during delay
                }
                continue;
            }

            try
            {
                _logger.Info("Performing system health check cycle.");
                await _auditLoggingAdapter.LogAuditEventAsync(
                    eventType: "MonitoringCycle",
                    eventDetails: "Starting new health check cycle.",
                    outcome: "Initiated",
                    sourceComponent: nameof(SystemHealthMonitorWorker));

                // 1. Aggregate Health Data
                var healthReport = await _healthAggregationService.AggregateHealthDataAsync(stoppingToken);
                _logger.Info($"Health aggregation completed. Overall status: {healthReport.OverallStatus}");
                // Optional: Log more details from the report if needed, e.g., specific component statuses

                // 2. Evaluate Alerts based on Health Report
                await _alertEvaluationService.EvaluateHealthReportAsync(healthReport, stoppingToken);
                _logger.Info("Alert evaluation cycle completed.");

                 await _auditLoggingAdapter.LogAuditEventAsync(
                    eventType: "MonitoringCycle",
                    eventDetails: $"Health check cycle completed. Overall status: {healthReport.OverallStatus}",
                    outcome: "Success",
                    sourceComponent: nameof(SystemHealthMonitorWorker));
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                // Expected exception when the service is stopping
                _logger.Info("SystemHealthMonitorWorker ExecuteAsync loop is stopping due to cancellation request.");
                break;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred during the system health monitoring cycle.");
                await _auditLoggingAdapter.LogAuditEventAsync(
                    eventType: "MonitoringCycle",
                    eventDetails: $"Error during health check cycle: {ex.Message}",
                    outcome: "Failure",
                    sourceComponent: nameof(SystemHealthMonitorWorker));
                // Depending on the severity of errors here, we might want a shorter delay before retrying
                // or a specific backoff strategy. For now, uses the standard interval.
            }

            // Wait for the next interval
            _logger.Info($"Health check cycle finished. Waiting for {_monitoringOptions.SystemHealthCheckInterval.TotalSeconds} seconds.");
            try
            {
                await Task.Delay(_monitoringOptions.SystemHealthCheckInterval, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                 _logger.Info("SystemHealthMonitorWorker stopping during delay.");
                break; // Exit loop if cancellation requested during delay
            }
        }
        await LogStopAsync();
    }

    private async Task LogStopAsync()
    {
        _logger.Info("SystemHealthMonitorWorker stopped.");
        await _auditLoggingAdapter.LogAuditEventAsync(
            eventType: "MonitoringWorker",
            eventDetails: "SystemHealthMonitorWorker stopping.",
            outcome: "Success",
            sourceComponent: nameof(SystemHealthMonitorWorker));
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.Info("SystemHealthMonitorWorker StopAsync called.");
        // Perform any cleanup tasks here if necessary
        await base.StopAsync(cancellationToken);
    }
}