namespace TheSSS.DICOMViewer.Monitoring.Contracts;

/// <summary>
/// Carries contextual information for an alert that has been triggered and needs to be evaluated for dispatch.
/// </summary>
/// <param name="TriggeredRuleName">The name of the rule that triggered this alert.</param>
/// <param name="AlertSeverity">The severity of the alert (e.g., "Information", "Warning", "Error", "Critical").</param>
/// <param name="Timestamp">The timestamp when the alert condition was met.</param>
/// <param name="SourceComponent">The component or system area that this alert pertains to (e.g., "Storage", "PACS", "License").</param>
/// <param name="Message">A human-readable summary message for the alert.</param>
/// <param name="RawData">The specific DTO or data object that triggered the alert (e.g., StorageHealthInfoDto, PacsConnectionInfoDto).</param>
/// <param name="AlertHash">A hash or unique signature for the alert, used for deduplication.</param>
public record AlertContextDto(
    string TriggeredRuleName,
    string AlertSeverity,
    DateTimeOffset Timestamp,
    string SourceComponent,
    string Message,
    object RawData,
    string AlertHash
);