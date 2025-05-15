namespace TheSSS.DICOMViewer.Monitoring.Contracts;

public enum AlertSeverity
{
    Info,
    Warning,
    Critical, // Use Critical for severe issues REQ-LDM-MNT-011
    Error // Can be an alias for Critical or a distinct level if needed. Let's align with schema.
          // Per file structure, "Critical", "Warning", "Info". Let's stick to these.
          // REQ-LDM-MNT-011 mentions 'critical issues', REQ-9-020 mentions 'critical system errors'.
}

public class AlertContextDto
{
    public string TriggeredRuleName { get; set; } = string.Empty;
    public AlertSeverity Severity { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string SourceComponent { get; set; } = string.Empty; // e.g., "StorageMonitor", "PacsMonitor-AE_TITLE_X"
    public string Message { get; set; } = string.Empty;
    public object? RawData { get; set; } // The specific DTO that triggered the alert (e.g., PacsConnectionInfoDto, StorageHealthInfoDto)
    public Guid AlertInstanceId { get; set; } = Guid.NewGuid(); // Unique ID for this specific alert instance
}