namespace TheSSS.DICOMViewer.Monitoring.Configuration;

public class AlertChannelSetting
{
    /// <summary>
    /// Gets or sets the type of the alert channel (e.g., "Email", "UI", "AuditLog").
    /// This must match the ChannelType property of a registered IAlertingChannel implementation.
    /// </summary>
    public string ChannelType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether this alert channel is enabled.
    /// Default is true.
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// Gets or sets recipient details specific to the channel (e.g., list of email addresses for the Email channel).
    /// </summary>
    public List<string>? RecipientDetails { get; set; }

    /// <summary>
    /// Gets or sets a list of alert severities that this channel should handle.
    /// If null or empty, the channel handles all severities.
    /// Values should match AlertSeverity enum names (e.g., "Critical", "Warning").
    /// </summary>
    public List<string>? Severities { get; set; }
}