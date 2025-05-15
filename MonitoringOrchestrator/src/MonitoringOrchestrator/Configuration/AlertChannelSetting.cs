using System.Collections.Generic;

namespace TheSSS.DICOMViewer.Monitoring.Configuration;

/// <summary>
/// Holds configuration settings for a specific alert channel (e.g., Email, UI).
/// </summary>
public class AlertChannelSetting
{
    /// <summary>
    /// Gets or sets the type of the alert channel (e.g., "Email", "UI", "AuditLog").
    /// This should match a registered IAlertingChannel implementation.
    /// </summary>
    public string ChannelType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether this alert channel is enabled.
    /// </summary>
    public bool IsEnabled { get; set; }

    /// <summary>
    /// Gets or sets a list of recipient email addresses. This is specific to the "Email" channel type.
    /// </summary>
    public List<string> RecipientEmailAddresses { get; set; } = new();

    /// <summary>
    /// Gets or sets the minimum severity level for alerts to be dispatched through this channel.
    /// Alerts with severity below this level will be ignored by this channel.
    /// If null or empty, all severities are dispatched (respecting IsEnabled).
    /// Example values: "Information", "Warning", "Error", "Critical".
    /// </summary>
    public string? MinimumSeverity { get; set; }
}