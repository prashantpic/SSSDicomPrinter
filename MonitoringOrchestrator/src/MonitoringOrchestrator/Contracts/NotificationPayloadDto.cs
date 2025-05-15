namespace TheSSS.DICOMViewer.Monitoring.Contracts;

public class NotificationPayloadDto
{
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public AlertSeverity Severity { get; set; }
    public DateTime Timestamp { get; set; }
    public string TargetChannelType { get; set; } = string.Empty;
    public List<string>? RecipientDetails { get; set; }
    public string SourceComponent { get; set; } = string.Empty;
    public string TriggeredRuleName { get; set; } = string.Empty;
}