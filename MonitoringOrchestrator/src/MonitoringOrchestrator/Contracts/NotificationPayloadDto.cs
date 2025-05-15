namespace TheSSS.DICOMViewer.Monitoring.Contracts;

public class NotificationPayloadDto
{
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public AlertSeverity Severity { get; set; } // Using the enum for consistency
    public DateTime Timestamp { get; set; }
    public string TargetChannelType { get; set; } = string.Empty; // e.g., "Email", "UI", "AuditLog"
    public List<string>? RecipientDetails { get; set; } // e.g., email addresses for email channel
    public string SourceComponent { get; set; } = string.Empty; // Original source of the health metric
    public string TriggeredRuleName { get; set; } = string.Empty; // Name of the rule that generated this alert
}