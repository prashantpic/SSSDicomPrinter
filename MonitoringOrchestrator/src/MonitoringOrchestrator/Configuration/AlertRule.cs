namespace TheSSS.DICOMViewer.Monitoring.Configuration;

public class AlertRule
{
    public string RuleName { get; set; } = string.Empty;
    public string MetricType { get; set; } = string.Empty;
    public string? SubMetricIdentifier { get; set; }
    public string ComparisonOperator { get; set; } = string.Empty;
    public double? ThresholdValue { get; set; }
    public string? ExpectedStatus { get; set; }
    public string Severity { get; set; } = "Warning";
    public int ConsecutiveFailuresToAlert { get; set; } = 1;
    public TimeSpan? ThrottleWindowOverride { get; set; }
    public TimeSpan? DeduplicationWindowOverride { get; set; }
}