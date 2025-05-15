using System;

namespace TheSSS.DICOMViewer.Security.DTOs
{
    /// <summary>
    /// Data transfer object for capturing detailed information about a security event to be recorded in the audit log.
    /// REQ-7-001, REQ-7-027
    /// </summary>
    /// <param name="EventType">The type of security event that occurred (e.g., "UserLogin", "PermissionCheck").</param>
    /// <param name="UserId">The identifier of the user associated with the event, if applicable.</param>
    /// <param name="Timestamp">The date and time when the event occurred.</param>
    /// <param name="Outcome">The outcome of the event (e.g., "Success", "Failure").</param>
    /// <param name="Details">Specific details about the event, potentially in a structured format like JSON. PHI should be masked.</param>
    /// <param name="SourceIP">The IP address from which the event originated, if applicable.</param>
    public record SecurityEventDetailsDto(
        string EventType,
        string? UserId,
        DateTimeOffset Timestamp,
        string Outcome,
        string Details,
        string? SourceIP);
}