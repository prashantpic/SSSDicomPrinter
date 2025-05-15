namespace TheSSS.DICOMViewer.Security.DTOs;

/// <summary>
/// Carries alert severity, message, source component, and optional error code.
/// Requirement REQ-LDM-LIC-005 (via IAlertingService).
/// </summary>
public record AlertDetailsDto(
    string Severity,
    string Message,
    string SourceComponent,
    string? ErrorCode = null
);