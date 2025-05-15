namespace TheSSS.DICOMViewer.Monitoring.Integrations;

using Prometheus;
using TheSSS.DICOMViewer.Monitoring.Contracts;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

public class PrometheusMetricsCollector
{
    private readonly ILogger<PrometheusMetricsCollector> _logger;

    // Define Prometheus metrics. Using const for metric names and help text.
    private const string MetricPrefix = "dicom_viewer_";

    private static readonly Gauge OverallHealthStatusGauge = Metrics
        .CreateGauge(MetricPrefix + "overall_health_status", "Overall system health status (0=Healthy, 1=Warning, 2=Error, 3=Critical, 4=Unknown)", "status_name");

    private static readonly Gauge StorageFreeBytesGauge = Metrics
        .CreateGauge(MetricPrefix + "storage_free_bytes", "Free space on the monitored storage location in bytes.", "storage_path_id");
    private static readonly Gauge StorageTotalBytesGauge = Metrics
        .CreateGauge(MetricPrefix + "storage_total_bytes", "Total capacity of the monitored storage location in bytes.", "storage_path_id");
    private static readonly Gauge StorageUsedPercentageGauge = Metrics
        .CreateGauge(MetricPrefix + "storage_used_percentage", "Percentage of storage space used on the monitored location.", "storage_path_id");

    private static readonly Gauge DatabaseConnectedGauge = Metrics
        .CreateGauge(MetricPrefix + "database_connected_status", "Database connectivity status (1=Connected, 0=Disconnected).", "database_identifier");
    private static readonly Gauge DatabaseLatencyMsGauge = Metrics
        .CreateGauge(MetricPrefix + "database_latency_ms", "Database query latency in milliseconds.", "database_identifier");

    private static readonly Gauge PacsNodeConnectedGauge = Metrics
        .CreateGauge(MetricPrefix + "pacs_node_connected_status", "PACS node connectivity status (1=Connected, 0=Disconnected).", "pacs_node_id");
    private static readonly Gauge PacsNodeConsecutiveFailuresGauge = Metrics
        .CreateGauge(MetricPrefix + "pacs_node_consecutive_failures", "PACS node consecutive failed echo checks.", "pacs_node_id");


    private static readonly Gauge LicenseValidGauge = Metrics
        .CreateGauge(MetricPrefix + "license_valid_status", "Application license validity (1=Valid, 0=Invalid).");
    private static readonly Gauge LicenseDaysUntilExpiryGauge = Metrics
        .CreateGauge(MetricPrefix + "license_days_until_expiry", "Days until application license expires. Negative if expired.");

    private static readonly Gauge SystemCriticalErrorCountGauge = Metrics
        .CreateGauge(MetricPrefix + "system_critical_error_count_24h", "Number of critical system errors in the last 24 hours.");

    private static readonly Gauge AutomatedTaskStatusGauge = Metrics
        .CreateGauge(MetricPrefix + "automated_task_status", "Status of automated tasks (0=Success, 1=Failed, 2=Running, 3=Unknown/NotRun).", "task_name");
    private static readonly Gauge AutomatedTaskLastRunAgeSecondsGauge = Metrics
        .CreateGauge(MetricPrefix + "automated_task_last_run_age_seconds", "Age of the last successful run of an automated task in seconds.", "task_name");


    public PrometheusMetricsCollector(ILogger<PrometheusMetricsCollector> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _logger.LogInformation("Prometheus Metrics Collector initialized.");
        // Metrics are registered globally by Prometheus.Client upon first creation.
    }

    /// <summary>
    /// Updates Prometheus metrics based on the provided HealthReportDto.
    /// This method should be called whenever a new health report is generated.
    /// </summary>
    /// <param name="report">The latest health report.</param>
    public void UpdateMetrics(HealthReportDto report)
    {
        if (report == null)
        {
            _logger.LogWarning("Attempted to update Prometheus metrics with a null report.");
            return;
        }
        _logger.LogDebug("Updating Prometheus metrics with latest health report data from {Timestamp}.", report.Timestamp);

        // Overall Health Status
        var overallStatusNumeric = report.OverallStatus switch
        {
            OverallHealthStatus.Healthy => 0,
            OverallHealthStatus.Warning => 1,
            OverallHealthStatus.Error => 2,
            OverallHealthStatus.Critical => 3, // Assuming Critical is worse than Error
            _ => 4 // Unknown
        };
        OverallHealthStatusGauge.WithLabels(report.OverallStatus.ToString()).Set(overallStatusNumeric);

        // Storage Metrics
        if (report.StorageHealth != null)
        {
            var storageId = report.StorageHealth.StoragePathIdentifier ?? "default_storage";
            StorageFreeBytesGauge.WithLabels(storageId).Set(report.StorageHealth.FreeSpaceBytes);
            StorageTotalBytesGauge.WithLabels(storageId).Set(report.StorageHealth.TotalCapacityBytes);
            StorageUsedPercentageGauge.WithLabels(storageId).Set(report.StorageHealth.UsedPercentage);
        }

        // Database Metrics
        if (report.DatabaseHealth != null)
        {
            var dbId = "application_db"; // Assuming a single DB for now
            DatabaseConnectedGauge.WithLabels(dbId).Set(report.DatabaseHealth.IsConnected ? 1 : 0);
            if (report.DatabaseHealth.LatencyMs.HasValue)
            {
                DatabaseLatencyMsGauge.WithLabels(dbId).Set(report.DatabaseHealth.LatencyMs.Value);
            }
        }

        // PACS Metrics
        if (report.PacsConnections != null)
        {
            // Clear old labels for PACS nodes that might no longer be configured or reported
            // This is complex; Prometheus.Client doesn't have an easy "clear labels not in this list".
            // One approach is to always set for all known PACS and set to a specific "stale" value if not in current report.
            // For simplicity, we'll just update what's in the report.
            foreach (var pacs in report.PacsConnections)
            {
                PacsNodeConnectedGauge.WithLabels(pacs.PacsNodeId).Set(pacs.IsConnected ? 1 : 0);
                PacsNodeConsecutiveFailuresGauge.WithLabels(pacs.PacsNodeId).Set(pacs.ConsecutiveFailedChecks);
            }
        }

        // License Metrics
        if (report.LicenseStatus != null)
        {
            LicenseValidGauge.Set(report.LicenseStatus.IsValid ? 1 : 0);
            LicenseDaysUntilExpiryGauge.Set(report.LicenseStatus.DaysUntilExpiry ?? -1); // Use -1 or NaN for unknown/not applicable
        }

        // System Error Metrics
        if (report.SystemErrorSummary != null)
        {
            SystemCriticalErrorCountGauge.Set(report.SystemErrorSummary.CriticalErrorCountLast24Hours);
            // Could iterate ErrorTypeSummaries and create dynamic gauges if needed (advanced)
        }

        // Automated Task Metrics
        if (report.AutomatedTaskStatuses != null)
        {
            var now = DateTime.UtcNow;
            foreach (var task in report.AutomatedTaskStatuses)
            {
                var taskStatusNumeric = task.LastRunStatus.ToLowerInvariant() switch
                {
                    "success" => 0,
                    "failed" => 1,
                    "running" => 2,
                    _ => 3 // Unknown/NotRun
                };
                AutomatedTaskStatusGauge.WithLabels(task.TaskName).Set(taskStatusNumeric);

                if (task.LastRunTimestamp.HasValue && task.LastRunStatus.Equals("success", StringComparison.OrdinalIgnoreCase))
                {
                    var ageSeconds = (now - task.LastRunTimestamp.Value).TotalSeconds;
                    AutomatedTaskLastRunAgeSecondsGauge.WithLabels(task.TaskName).Set(ageSeconds);
                }
                else
                {
                     // If not run or failed, age could be set to a very high value or a special indicator (-1)
                     AutomatedTaskLastRunAgeSecondsGauge.WithLabels(task.TaskName).Set(-1);
                }
            }
        }

        _logger.LogDebug("Prometheus metrics updated successfully.");
    }
}