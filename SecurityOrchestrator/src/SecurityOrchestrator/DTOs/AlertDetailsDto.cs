namespace TheSSS.DICOMViewer.Security.DTOs
{
    /// <summary>
    /// Data transfer object carrying information for an alert to be raised, typically for administrative notification.
    /// REQ-LDM-LIC-005
    /// </summary>
    /// <param name="Severity">The severity level of the alert (e.g., "Critical", "Warning", "Information").</param>
    /// <param name="Message">The detailed message describing the alert.</param>
    /// <param name="SourceComponent">The component or module that generated the alert.</param>
    /// <param name="ErrorCode">An optional error code associated with the alert.</param>
    public record AlertDetailsDto(
        string Severity,
        string Message,
        string SourceComponent,
        string? ErrorCode);
}