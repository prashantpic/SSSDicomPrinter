namespace TheSSS.DICOMViewer.Monitoring.Configuration;

public class AlertRule
{
    /// <summary>
    /// Gets or sets the unique name for this alert rule.
    /// </summary>
    public string RuleName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the type of metric this rule evaluates (e.g., "StorageUsagePercent", "PacsConnectivity", "LicenseStatus").
    /// This should correspond to properties or data types in HealthReportDto or its constituent DTOs.
    /// </summary>
    public string MetricType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets an optional identifier for a sub-metric, if MetricType refers to a collection
    /// (e.g., AETitle for PacsConnectivity, TaskName for AutomatedTaskStatus).
    /// </summary>
    public string? SubMetricIdentifier { get; set; }

    /// <summary>
    /// Gets or sets the threshold value for numeric metrics.
    /// </summary>
    public double? ThresholdValue { get; set; }

    /// <summary>
    /// Gets or sets the comparison operator (e.g., "GreaterThan", "LessThan", "EqualTo", "NotEqualTo", "Contains").
    /// </summary>
    public string ComparisonOperator { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the expected status for string or boolean metrics (e.g., "False" for IsConnected, "Failed" for TaskStatus).
    /// </summary>
    public string? ExpectedStatus { get; set; }

    /// <summary>
    /// Gets or sets the severity of the alert if triggered (e.g., "Critical", "Warning", "Info").
    /// Should match values from AlertSeverity enum.
    /// </summary>
    public string Severity { get; set; } = "Warning";

    /// <summary>
    /// Gets or sets the number of consecutive times the condition must be met before triggering an alert.
    /// Default is 1. Useful for transient issues like network blips.
    /// </summary>
    public int ConsecutiveFailuresToAlert { get; set; } = 1;

    /// <summary>
    /// Optional override for the default alert throttling window for this specific rule.
    /// </summary>
    public TimeSpan? ThrottleWindowOverride { get; set; }

    /// <summary>
    /// Optional override for the default alert deduplication window for this specific rule.
    /// </summary>
    public TimeSpan? DeduplicationWindowOverride { get; set; }
}