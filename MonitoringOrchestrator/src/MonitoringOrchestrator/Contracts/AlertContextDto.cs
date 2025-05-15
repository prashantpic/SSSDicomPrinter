using System;

namespace TheSSS.DICOMViewer.Monitoring.Contracts;

/// <summary>
/// Data Transfer Object carrying context for an alert to be evaluated and dispatched.
/// </summary>
public class AlertContextDto
{
    /// <summary>
    /// The name of the alert rule that was triggered.
    /// </summary>
    public string TriggeredRuleName { get; set; } = string.Empty;

    /// <summary>
    /// The severity level of the alert (e.g., 'Critical', 'Warning', 'Info').
    /// Should align with configured severities in AlertRule.
    /// </summary>
    public string AlertSeverity { get; set; } = "Warning";

    /// <summary>
    /// Timestamp when the alert condition was detected.
    /// </summary>
    public DateTimeOffset Timestamp { get; set; }

    /// <summary>
    /// The component or area of the system that is the source of the alert
    /// (e.g., "Storage", "PACS:AETITLE_XYZ", "Database", "License", "AutomatedTask:DataPurge").
    /// </summary>
    public string SourceComponent { get; set; } = string.Empty;

    /// <summary>
    /// A human-readable message summarizing the alert.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Optional raw data object containing the specific health DTO (e.g., StorageHealthInfoDto, PacsConnectionInfoDto)
    /// or event data that triggered the alert. This can be used for detailed reporting or troubleshooting.
    /// </summary>
    public object? RawData { get; set; }

    /// <summary>
    /// Optional: Unique identifier for correlating this alert instance across different systems or logs.
    /// </summary>
    public Guid CorrelationId { get; set; } = Guid.NewGuid();
}