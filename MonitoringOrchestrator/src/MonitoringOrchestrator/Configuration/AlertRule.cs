using System.ComponentModel.DataAnnotations;

namespace TheSSS.DICOMViewer.Monitoring.Configuration;

/// <summary>
/// POCO class representing a single alert rule definition.
/// </summary>
public class AlertRule
{
    /// <summary>
    /// Unique name for the alert rule (e.g., "HighStorageUsage", "PACS_Offline_AETITLE").
    /// </summary>
    [Required(ErrorMessage = "RuleName is required.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "RuleName must be between 3 and 100 characters.")]
    public string RuleName { get; set; } = string.Empty;

    /// <summary>
    /// The type of metric or event this rule monitors (e.g., "StorageUsagePercent", "PacsConnectivity", "LicenseStatus", "CriticalErrorCount").
    /// This string is used by AlertEvaluationService to select the correct logic.
    /// </summary>
    [Required(ErrorMessage = "MetricType is required.")]
    public string MetricType { get; set; } = string.Empty;

    /// <summary>
    /// The threshold value to compare against the metric. Interpretation depends on MetricType and ComparisonOperator.
    /// </summary>
    public double ThresholdValue { get; set; } // No [Required] as some rules might not need a threshold (e.g., boolean status checks)

    /// <summary>
    /// The comparison operator to use (e.g., "GreaterThan", "EqualTo", "LessThan", "NotEqualTo", "BecomesFalse", "BecomesTrue").
    /// </summary>
    [Required(ErrorMessage = "ComparisonOperator is required.")]
    public string ComparisonOperator { get; set; } = string.Empty;

    /// <summary>
    /// The severity level of the alert if this rule triggers (e.g., "Critical", "Warning", "Info").
    /// </summary>
    [Required(ErrorMessage = "Severity is required.")]
    public string Severity { get; set; } = "Warning";

    /// <summary>
    /// For metrics that check status over time (like connectivity), the number of consecutive
    /// failures/matches required to trigger the alert. Default is 1 (trigger on first match).
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "ConsecutiveFailuresToAlert must be at least 1.")]
    public int ConsecutiveFailuresToAlert { get; set; } = 1;

    /// <summary>
    /// Optional: A specific instance identifier this rule applies to (e.g., a PACS AE Title for a "PacsConnectivity" rule,
    /// or a specific task name for "AutomatedTaskStatus"). If null/empty, the rule may apply globally for the MetricType.
    /// </summary>
    public string? InstanceIdentifier { get; set; }

    /// <summary>
    /// Indicates if this rule is enabled. Disabled rules are ignored by the AlertEvaluationService.
    /// </summary>
    public bool Enabled { get; set; } = true;
}