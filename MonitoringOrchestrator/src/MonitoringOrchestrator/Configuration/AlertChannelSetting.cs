using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TheSSS.DICOMViewer.Monitoring.Configuration;

/// <summary>
/// POCO class for settings related to a specific alert channel.
/// </summary>
public class AlertChannelSetting
{
    /// <summary>
    /// The type of channel (e.g., "Email", "UI", "AuditLog").
    /// This string is used by AlertDispatchService to find the correct IAlertingChannel implementation.
    /// </summary>
    [Required(ErrorMessage = "ChannelType is required.")]
    public string ChannelType { get; set; } = string.Empty;

    /// <summary>
    /// Indicates if this channel is enabled for dispatching alerts.
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// List of recipient email addresses (used only if ChannelType is "Email").
    /// If ChannelType is "Email" and IsEnabled is true, this list must not be empty and contain valid emails.
    /// Validation for this is handled by AlertingOptionsValidator.
    /// </summary>
    public List<string> RecipientEmailAddresses { get; set; } = new List<string>();

    /// <summary>
    /// Optional: Minimum severity level for alerts to be dispatched via this channel
    /// (e.g., "Warning", "Critical"). If an alert's severity is below this, it won't be sent through this channel.
    /// Defaults to "Info" or "Warning" effectively if not set or handled (meaning all severities pass).
    /// </summary>
    public string? MinimumSeverity { get; set; } // e.g., "Warning", "Critical". Allows channel-specific filtering.
}