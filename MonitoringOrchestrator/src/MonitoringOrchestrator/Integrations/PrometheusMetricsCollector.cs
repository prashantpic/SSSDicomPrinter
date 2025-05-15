```csharp
using Microsoft.Extensions.Logging;
using Prometheus;
using TheSSS.DICOMViewer.Monitoring.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TheSSS.DICOMViewer.Monitoring.Integrations
{
    /// <summary>
    /// Service for collecting and exposing metrics via Prometheus.NET.
    /// Exposes internal health metrics in a format consumable by Prometheus for external monitoring dashboards.
    /// </summary>
    public class PrometheusMetricsCollector
    {
        private readonly ILogger<PrometheusMetricsCollector> _logger;

        private static readonly Gauge DicomViewerSystemOverallStatus = Metrics
            .CreateGauge("dicomviewer_system_overall_status", "Overall health status of the DICOM Viewer system (1=Healthy, 2=Warning, 3=Error).");

        // Storage Metrics
        private static readonly Gauge DicomViewerStorageFreeBytes = Metrics
            .CreateGauge("dicomviewer_storage_free_bytes", "Free disk space in bytes for monitored paths.", "path");
        private static readonly Gauge DicomViewerStorageTotalCapacityBytes = Metrics
            .CreateGauge("dicomviewer_storage_total_capacity_bytes", "Total disk capacity in bytes for monitored paths.", "path");
        private static readonly Gauge DicomViewerStorageUsedPercentage = Metrics
            .CreateGauge("dicomviewer_storage_used_percent", "Percentage of disk space used for monitored paths.", "path");

        // Database Metrics
        private static readonly Gauge DicomViewerDbConnected = Metrics
            .CreateGauge("dicomviewer_db_connected", "Database connectivity status (1 if connected, 0 if not).");
        private static readonly Gauge DicomViewerDbLatencyMs = Metrics
            .CreateGauge("dicomviewer_db_latency_ms", "Database query latency in milliseconds.");

        // PACS Metrics
        private static readonly Gauge DicomViewerPacsConnected = Metrics
            .CreateGauge("dicomviewer_pacs_connected", "PACS node connectivity status (1 if connected, 0 if not).", "aetitle");

        // License Metrics
        private static readonly Gauge DicomViewerLicenseValid = Metrics
            .CreateGauge("dicomviewer_license_valid", "License validity status (1 if valid, 0 if invalid).");
        private static readonly Gauge DicomViewerLicenseDaysUntilExpiry = Metrics
            .CreateGauge("dicomviewer_license_days_until_expiry", "Number of days until license expiry.");

        // System Error Metrics
        private static readonly Gauge DicomViewerCriticalErrorCount24h = Metrics
            .CreateGauge("dicomviewer_critical_error_count_24h", "Number of critical errors in the last 24 hours.");
        private static readonly Gauge DicomViewerErrorTypeCount = Metrics
            .CreateGauge("dicomviewer_error_type_count_24h", "Count of specific error types in the last 24 hours.", "error_type");
            
        // Automated Task Metrics
        private static readonly Gauge DicomViewerAutomatedTaskStatus = Metrics
            .CreateGauge("dicomviewer_automated_task_status", "Status of automated tasks (1 for success, 0 for failure, 2 for running, 3 for unknown).", "task_name");
        private static readonly Gauge DicomViewerAutomatedTaskLastRunTimestamp = Metrics
            .CreateGauge("dicomviewer_automated_task_last_run_timestamp_seconds", "Unix timestamp of the last run for an automated task.", "task_name");


        // Alerting Metrics
        private static readonly Counter DicomViewerAlertTriggeredTotal = Metrics
            .CreateCounter("dicomviewer_alert_triggered_total", "Total number of alerts triggered.", "rule_name", "severity");
        private static readonly Counter DicomViewerAlertDispatchedTotal = Metrics
            .CreateCounter("dicomviewer_alert_dispatched_total", "Total number of alerts dispatched.", "rule_name", "severity", "channel_type");
        private static readonly Counter DicomViewerAlertThrottledTotal = Metrics
            .CreateCounter("dicomviewer_alert_throttled_total", "Total number of alerts throttled.", "rule_name", "severity");
        private static readonly Counter DicomViewerAlertDeduplicatedTotal = Metrics
            .CreateCounter("dicomviewer_alert_deduplicated_total", "Total number of alerts deduplicated.", "rule_name", "severity");


        /// <summary>
        /// Initializes a new instance of the <see cref="PrometheusMetricsCollector"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        public PrometheusMetricsCollector(ILogger<PrometheusMetricsCollector> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _logger.LogInformation("PrometheusMetricsCollector initialized.");
        }

        /// <summary>
        /// Updates all relevant Prometheus metrics based on the provided <see cref="HealthReportDto"/>.
        /// </summary>
        /// <param name="report">The aggregated health report.</param>
        public void UpdateMetrics(HealthReportDto report)
        {
            if (report == null)
            {
                _logger.LogWarning("Received null HealthReportDto. Skipping Prometheus metrics update.");
                return;
            }

            _logger.LogDebug("Updating Prometheus metrics from HealthReportDto timestamped {Timestamp}.", report.Timestamp);

            DicomViewerSystemOverallStatus.Set((double)report.SystemStatus);

            if (report.StorageInfo != null)
            {
                UpdateStorageMetrics(report.StorageInfo);
            }

            if (report.DatabaseConnectivity != null)
            {
                UpdateDatabaseMetrics(report.DatabaseConnectivity);
            }

            if (report.PacsStatuses != null)
            {
                UpdatePacsMetrics(report.PacsStatuses);
            }

            if (report.LicenseStatus != null)
            {
                UpdateLicenseMetrics(report.LicenseStatus);
            }

            if (report.SystemErrorSummary != null)
            {
                UpdateSystemErrorMetrics(report.SystemErrorSummary);
            }

            if (report.AutomatedTaskStatuses != null)
            {
                UpdateAutomatedTaskMetrics(report.AutomatedTaskStatuses);
            }
            _logger.LogDebug("Prometheus metrics update complete.");
        }

        /// <summary>
        /// Updates storage-related metrics.
        /// </summary>
        /// <param name="storageInfo">The storage health information.</param>
        public void UpdateStorageMetrics(StorageHealthInfoDto storageInfo)
        {
            if (string.IsNullOrEmpty(storageInfo.Path))
            {
                _logger.LogWarning("Storage path is null or empty, cannot update path-specific storage metrics.");
                return;
            }
            DicomViewerStorageFreeBytes.WithLabels(storageInfo.Path).Set(storageInfo.FreeSpaceBytes);
            DicomViewerStorageTotalCapacityBytes.WithLabels(storageInfo.Path).Set(storageInfo.TotalCapacityBytes);
            DicomViewerStorageUsedPercentage.WithLabels(storageInfo.Path).Set(storageInfo.UsedPercentage);
        }

        /// <summary>
        /// Updates database connectivity metrics.
        /// </summary>
        /// <param name="dbInfo">The database connectivity information.</param>
        public void UpdateDatabaseMetrics(DatabaseConnectivityInfoDto dbInfo)
        {
            DicomViewerDbConnected.Set(dbInfo.IsConnected ? 1 : 0);
            if (dbInfo.LatencyMs.HasValue)
            {
                DicomViewerDbLatencyMs.Set(dbInfo.LatencyMs.Value);
            }
            else
            {
                 DicomViewerDbLatencyMs.Set(0); // Or some indicator for unknown latency
            }
        }

        /// <summary>
        /// Updates PACS connectivity metrics.
        /// </summary>
        /// <param name="pacsStatuses">A list of PACS connection information.</param>
        public void UpdatePacsMetrics(IEnumerable<PacsConnectionInfoDto> pacsStatuses)
        {
            // Clear old labels if PACS nodes can change dynamically.
            // For simplicity, this example assumes a relatively static set of PACS nodes or that unreferenced labels will eventually expire.
            // Prometheus.Client has some mechanisms for this, or one might need to track active labels.
            // DicomViewerPacsConnected.Reset(); // This clears all labels, might not be ideal if some PACS are still valid.

            foreach (var pacs in pacsStatuses)
            {
                if (string.IsNullOrEmpty(pacs.PacsNodeId))
                {
                     _logger.LogWarning("PACS Node ID is null or empty, cannot update PACS connectivity metric for this node.");
                    continue;
                }
                DicomViewerPacsConnected.WithLabels(pacs.PacsNodeId).Set(pacs.IsConnected ? 1 : 0);
            }
        }

        /// <summary>
        /// Updates license status metrics.
        /// </summary>
        /// <param name="licenseInfo">The license status information.</param>
        public void UpdateLicenseMetrics(LicenseStatusInfoDto licenseInfo)
        {
            DicomViewerLicenseValid.Set(licenseInfo.IsValid ? 1 : 0);
            DicomViewerLicenseDaysUntilExpiry.Set(licenseInfo.DaysUntilExpiry ?? -1); // -1 for N/A
        }
        
        /// <summary>
        /// Updates system error summary metrics.
        /// </summary>
        /// <param name="errorSummary">The system error summary information.</param>
        public void UpdateSystemErrorMetrics(SystemErrorInfoSummaryDto errorSummary)
        {
            DicomViewerCriticalErrorCount24h.Set(errorSummary.CriticalErrorCountLast24Hours);
            if (errorSummary.ErrorTypeSummaries != null)
            {
                // Consider clearing old error type labels if they are dynamic
                // DicomViewerErrorTypeCount.Reset(); 
                foreach (var summary in errorSummary.ErrorTypeSummaries)
                {
                    if(string.IsNullOrEmpty(summary.Type)) continue;
                    DicomViewerErrorTypeCount.WithLabels(summary.Type).Set(summary.Count);
                }
            }
        }

        /// <summary>
        /// Updates automated task status metrics.
        /// </summary>
        /// <param name="taskStatuses">A list of automated task status information.</param>
        public void UpdateAutomatedTaskMetrics(IEnumerable<AutomatedTaskStatusInfoDto> taskStatuses)
        {
            // Consider clearing old task labels if tasks are dynamic
            // DicomViewerAutomatedTaskStatus.Reset();
            // DicomViewerAutomatedTaskLastRunTimestamp.Reset();
            foreach (var task in taskStatuses)
            {
                if(string.IsNullOrEmpty(task.TaskName)) continue;

                double statusValue = task.LastRunStatus?.ToLowerInvariant() switch
                {
                    "success" => 1,
                    "failed" => 0,
                    "running" => 2,
                    _ => 3 // Unknown
                };
                DicomViewerAutomatedTaskStatus.WithLabels(task.TaskName).Set(statusValue);

                if (task.LastRunTimestamp.HasValue)
                {
                    DicomViewerAutomatedTaskLastRunTimestamp.WithLabels(task.TaskName).Set(task.LastRunTimestamp.Value.ToUnixTimeSeconds());
                }
            }
        }

        /// <summary>
        /// Increments the counter for triggered alerts.
        /// </summary>
        /// <param name="ruleName">The name of the triggered rule.</param>
        /// <param name="severity">The severity of the alert.</param>
        public void IncrementAlertTriggered(string ruleName, string severity)
        {
            DicomViewerAlertTriggeredTotal.WithLabels(ruleName, severity).Inc();
        }

        /// <summary>
        /// Increments the counter for dispatched alerts.
        /// </summary>
        /// <param name="ruleName">The name of the rule for the dispatched alert.</param>
        /// <param name="severity">The severity of the alert.</param>
        /// <param name="channelType">The type of channel the alert was dispatched through.</param>
        public void IncrementAlertDispatched(string ruleName, string severity, string channelType)
        {
            DicomViewerAlertDispatchedTotal.WithLabels(ruleName, severity, channelType).Inc();
        }

        /// <summary>
        /// Increments the counter for throttled alerts.
        /// </summary>
        /// <param name="ruleName">The name of the rule for the throttled alert.</param>
        /// <param name="severity">The severity of the alert.</param>
        public void IncrementAlertThrottled(string ruleName, string severity)
        {
            DicomViewerAlertThrottledTotal.WithLabels(ruleName, severity).Inc();
        }

        /// <summary>
        /// Increments the counter for deduplicated alerts.
        /// </summary>
        /// <param name="ruleName">The name of the rule for the deduplicated alert.</param>
        /// <param name="severity">The severity of the alert.</param>
        public void IncrementAlertDeduplicated(string ruleName, string severity)
        {
            DicomViewerAlertDeduplicatedTotal.WithLabels(ruleName, severity).Inc();
        }
    }
}
```