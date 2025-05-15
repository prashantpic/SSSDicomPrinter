namespace TheSSS.DICOMViewer.Monitoring.Contracts;

public enum AlertSeverity
{
    Info,
    Warning,
    Critical
}

public class AlertContextDto
{
    public string TriggeredRuleName { get; set; } = string.Empty;
    public AlertSeverity Severity { get; set; }
    public DateTime Timestamp { get; set; }
    public string SourceComponent { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public object? RawData { get; set; }
    public Guid AlertInstanceId { get; set; } = Guid.NewGuid();
}