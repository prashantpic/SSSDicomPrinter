using Prometheus;
using TheSSS.DICOMViewer.Monitoring.Contracts;
using TheSSS.DICOMViewer.Monitoring.Interfaces.Adapters;

namespace TheSSS.DICOMViewer.Monitoring.Integrations;

public class PrometheusMetricsCollector
{
    private readonly ILoggerAdapter<PrometheusMetricsCollector> _logger;

    // --- Metric Definitions ---
    // Using "_" as separator and "total" suffix for counters is Prometheus best practice.
    // Using "bytes", "seconds", "ratio" for units where applicable.

    private static readonly Gauge DicomViewerStorageTotalBytes = Metrics.CreateGauge(
        "dicomviewer_storage_total_bytes",
        "Total storage capacity on the DICOM repository volume in bytes.",
        new GaugeConfiguration { LabelNames = new[] { "storage_id" }, SuppressInitialValue = true }
    );

    private static readonly Gauge DicomViewerStorageFreeBytes = Metrics.CreateGauge(
        "dicomviewer_storage_free_bytes",
        "Free storage space on the DICOM repository volume in bytes.",
        new GaugeConfiguration { LabelNames = new[] { "storage_id" }, SuppressInitialValue = true }
    );

    private static readonly Gauge DicomViewerStorageUsedRatio = Metrics.CreateGauge(
        "dicomviewer_storage_used_ratio",
        "Ratio of storage space used on the DICOM repository volume (0.0 to 1.0).",
        new GaugeConfiguration { LabelNames = new[] { "storage_id" }, SuppressInitialValue = true }
    );

    private static readonly Gauge DicomViewerDatabaseConnectedStatus = Metrics.CreateGauge(
        "dicomviewer_database_connected_status",
        "Database connectivity status (1 for connected, 0 for disconnected).",
        new GaugeConfiguration { SuppressInitialValue = true }
    );

    private static readonly Gauge DicomViewerDatabaseLatencySeconds = Metrics.CreateGauge(
        "dicomviewer_database_latency_seconds",
        "Database connectivity check latency in seconds.",
        new GaugeConfiguration { SuppressInitialValue = true }
    );

    private static readonly Gauge DicomViewerPacsNodeConnectedStatus = Metrics.CreateGauge(
        "dicomviewer_pacs_node_connected_status",
        "PACS node connectivity status (1 for connected, 0 for disconnected).",
        new GaugeConfiguration { LabelNames = new[] { "aetitle" }, SuppressInitialValue = true }
    );

    private static readonly Gauge DicomViewerLicenseValidStatus = Metrics.CreateGauge(
        "dicomviewer_license_valid_status",
        "Application license validity status (1 for valid, 0 for invalid).",
        new GaugeConfiguration { SuppressInitialValue = true }
    );

    private static readonly Gauge DicomViewerLicenseDaysUntilExpiry = Metrics.CreateGauge(
        "dicomviewer_license_days_until_expiry",
        "Number of days until the application license expires. (-1 if not applicable/invalid).",
        new GaugeConfiguration { SuppressInitialValue = true }
    );

    private static readonly Gauge DicomViewerCriticalErrorCount = Metrics.CreateGauge(
        "dicomviewer_critical_error_count",
        "Number of critical system errors logged in the configured lookback period.",
        new GaugeConfiguration { SuppressInitialValue = true }
    );

    private static readonly Gauge DicomViewerAutomatedTaskLastRunStatus = Metrics.CreateGauge(
        "dicomviewer_automated_task_last_run_status",
        "Status of the last automated task run (1 for Success, 0 for Failed, 2 for Running, 3 for Unknown/Skipped).",
        new GaugeConfiguration { LabelNames = new[] { "task_name" }, SuppressInitialValue = true }
    );
    
    private static readonly Gauge DicomViewerAutomatedTaskLastRunTimestampSeconds = Metrics.CreateGauge(
        "dicomviewer_automated_task_last_run_timestamp_seconds",
        "Unix timestamp of the last recorded run time for automated tasks.",
        new GaugeConfiguration { LabelNames = new[] { "task_name" }, SuppressInitialValue = true }
    );

    private static readonly Gauge DicomViewerOverallHealthStatus = Metrics.CreateGauge(
        "dicomviewer_overall_health_status",
        "Overall system health status (0=Healthy, 1=Warning, 2=Degraded, 3=Error).",
         new GaugeConfiguration { SuppressInitialValue = true }
    );


    public PrometheusMetricsCollector(ILoggerAdapter<PrometheusMetricsCollector> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _logger.Info("PrometheusMetricsCollector initialized. Metrics are statically registered.");
    }

    public void UpdateMetrics(HealthReportDto report)
    {
        if (report == null)
        {
            _logger.Warning("UpdateMetrics called with a null health report. Skipping metrics update.");
            return;
        }

        _logger.Debug("Updating Prometheus metrics with latest health report.");
        try
        {
            // Overall Health Status
            DicomViewerOverallHealthStatus.Set(report.OverallStatus switch
            {
                "Healthy" => 0,
                "Warning" => 1,
                "Degraded" => 2,
                "Error" => 3,
                _ => 4 // Unknown
            });

            // Storage Info
            if (report.StorageInfo != null)
            {
                string storageId = report.StorageInfo.StorageIdentifier ?? "default";
                DicomViewerStorageTotalBytes.WithLabels(storageId).Set(report.StorageInfo.TotalCapacityBytes);
                DicomViewerStorageFreeBytes.WithLabels(storageId).Set(report.StorageInfo.FreeSpaceBytes);
                DicomViewerStorageUsedRatio.WithLabels(storageId).Set(report.StorageInfo.TotalCapacityBytes > 0 ? (double)(report.StorageInfo.TotalCapacityBytes - report.StorageInfo.FreeSpaceBytes) / report.StorageInfo.TotalCapacityBytes : 0);
            }
            else
            {
                _logger.Debug("Storage health info missing in report for metrics update. Metrics may not be set or may become stale.");
                // Consider setting to NaN or specific values if a source is explicitly unavailable
                 // DicomViewerStorageTotalBytes.WithLabels("default").Set(double.NaN); // Example for missing data
            }

            // Database Connectivity
            if (report.DatabaseConnectivity != null)
            {
                DicomViewerDatabaseConnectedStatus.Set(report.DatabaseConnectivity.IsConnected ? 1 : 0);
                DicomViewerDatabaseLatencySeconds.Set(report.DatabaseConnectivity.LatencyMs.HasValue ? report.DatabaseConnectivity.LatencyMs.Value / 1000.0 : -1); // -1 if no latency
            }
            else
            {
                 _logger.Debug("Database connectivity info missing in report. Metrics may become stale.");
                 // DicomViewerDatabaseConnectedStatus.Set(double.NaN);
            }

            // PACS Connections
            // To handle removed PACS nodes, we might need a list of all *configured* nodes.
            // For now, only update reported ones. Stale metrics for removed nodes can be an issue.
            // One strategy: unpublish metrics for nodes not in the current report if we know all configured nodes.
            if (report.PacsConnections != null)
            {
                foreach (var pacs in report.PacsConnections)
                {
                    if (!string.IsNullOrEmpty(pacs.PacsNodeId))
                    {
                        DicomViewerPacsNodeConnectedStatus.WithLabels(pacs.PacsNodeId).Set(pacs.IsConnected ? 1 : 0);
                    }
                }
            }
            else
            {
                _logger.Debug("PACS connections info missing in report. Metrics may become stale.");
            }


            // License Status
            if (report.LicenseStatus != null)
            {
                DicomViewerLicenseValidStatus.Set(report.LicenseStatus.IsValid ? 1 : 0);
                DicomViewerLicenseDaysUntilExpiry.Set(report.LicenseStatus.DaysUntilExpiry ?? -1);
            }
            else
            {
                 _logger.Debug("License status info missing in report. Metrics may become stale.");
                 // DicomViewerLicenseValidStatus.Set(double.NaN);
            }


            // System Error Summary
            if (report.SystemErrorSummary != null)
            {
                DicomViewerCriticalErrorCount.Set(report.SystemErrorSummary.CriticalErrorCountLast24Hours);
            }
            else
            {
                 _logger.Debug("System error summary info missing in report. Metrics may become stale.");
                 // DicomViewerCriticalErrorCount.Set(double.NaN);
            }

            // Automated Task Statuses
            if (report.AutomatedTaskStatuses != null)
            {
                foreach (var task in report.AutomatedTaskStatuses)
                {
                    if (!string.IsNullOrEmpty(task.TaskName))
                    {
                        DicomViewerAutomatedTaskLastRunStatus.WithLabels(task.TaskName).Set(task.LastRunStatus switch
                        {
                            "Success" => 1,
                            "Failed" => 0,
                            "Running" => 2,
                            _ => 3 // Unknown/Skipped
                        });
                        if (task.LastRunTimestamp.HasValue)
                        {
                            DicomViewerAutomatedTaskLastRunTimestampSeconds.WithLabels(task.TaskName).Set(task.LastRunTimestamp.Value.ToUnixTimeSeconds());
                        }
                    }
                }
            }
            else
            {
                _logger.Debug("Automated task statuses info missing in report. Metrics may become stale.");
            }

            _logger.Debug("Prometheus metrics update process finished.");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "An unexpected error occurred while updating Prometheus metrics.");
            // Do not re-throw; metrics collection failure should not stop monitoring.
        }
    }
}