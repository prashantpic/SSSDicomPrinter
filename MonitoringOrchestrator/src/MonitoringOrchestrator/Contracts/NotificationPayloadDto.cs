using System.Collections.Generic;

namespace TheSSS.DICOMViewer.Monitoring.Contracts;

/// <summary>
/// Represents the formatted payload to be sent to an alerting channel.
/// </summary>
/// <param name="Title">The title of the notification.</param>
/// <param name="Body">The main content/body of the notification, formatted for human consumption.</param>
/// <param name="Severity">The severity of the alert (e.g., "Information", "Warning", "Error", "Critical").</param>
/// <param name="Timestamp">The timestamp of the original alert event.</param>
/// <param name="TargetChannelType">The type of channel this payload is intended for (e.g., "Email", "UI", "AuditLog").</param>
/// <param name="RecipientDetails">A list of recipient identifiers (e.g., email addresses), if applicable to the channel.</param>
/// <param name="SourceComponent">The component or system area that this alert pertains to.</param>
public record NotificationPayloadDto(
    string Title,
    string Body,
    string Severity,
    DateTimeOffset Timestamp,
    string TargetChannelType,
    List<string>? RecipientDetails,
    string SourceComponent
);