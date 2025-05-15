namespace TheSSS.DICOMViewer.Monitoring.Configuration;

/// <summary>
/// Represents a single alert rule definition, specifying conditions for triggering an alert.
/// </summary>
public class AlertRule
{
    /// <summary>
    /// Gets or sets the unique name for this alert rule.
    /// </summary>
    public string RuleName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the type of metric this rule applies to (e.g., "StorageUsagePercent", "PacsConnectivity").
    /// This should correspond to a property or data point in the HealthReportDto or its constituent DTOs.
    /// </summary>
    public string MetricType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the threshold value that the metric will be compared against.
    /// </summary>
    public double ThresholdValue { get; set; }

    /// <summary>
    /// Gets or sets the comparison operator to use (e.g., "GreaterThan", "LessThan", "EqualTo").
    /// </summary>
    public string ComparisonOperator { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the severity of the alert if triggered (e.g., "Information", "Warning", "Error", "Critical").
    /// </summary>
    public string Severity { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the number of consecutive failures or checks meeting the criteria before an alert is triggered.
    /// A value of 1 means an alert is triggered on the first occurrence.
    /// </summary>
    public int ConsecutiveFailuresToAlert { get; set; } = 1;

    /// <summary>
    /// Gets or sets an optional identifier for rules that are specific to a particular target 
    /// (e.g., a specific PACS node AETitle or a storage path).
    /// If null or empty, the rule applies globally to the MetricType.
    /// </summary>
    public string? TargetIdentifier { get; set; }
}