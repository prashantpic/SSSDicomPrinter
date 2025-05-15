using System;
using System.Collections.Generic;

namespace TheSSS.DICOMViewer.Monitoring.Contracts;

/// <summary>
/// DTO representing the payload sent to an alerting channel.
/// </summary>
public class NotificationPayloadDto
{
    /// <summary>
    /// The title or subject line for the notification (e.g., "Critical Alert: Storage Full").
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// The main body of the notification message, potentially formatted with details.
    /// </summary>
    public string Body { get; set; } = string.Empty;

    /// <summary>
    /// The severity level of the alert (e.g., 'Critical', 'Warning', 'Info').
    /// Matches AlertContextDto.AlertSeverity.
    /// </summary>
    public string Severity { get; set; } = "Warning";

    /// <summary>
    /// Timestamp of the original alert.
    /// </summary>
    public DateTimeOffset Timestamp { get; set; }

    /// <summary>
    /// Indicates the intended channel type for this payload (e.g., "Email", "UI", "AuditLog").
    /// This helps the dispatch service or the channel itself in handling the payload.
    /// This is set by AlertDispatchService before calling the channel.
    /// </summary>
    public string TargetChannelType { get; set; } = string.Empty;

    /// <summary>
    /// Specific recipient details, if applicable to the channel.
    /// For "Email", this would be a `List<string>` of email addresses.
    /// For "UI", this might be null or contain user/session identifiers.
    /// This is set by AlertDispatchService based on AlertChannelSetting.
    /// </summary>
    public object? RecipientDetails { get; set; }

    /// <summary>
    /// Optional: Correlation ID from the original AlertContextDto for tracking.
    /// </summary>
    public Guid? CorrelationId { get; set; }
}