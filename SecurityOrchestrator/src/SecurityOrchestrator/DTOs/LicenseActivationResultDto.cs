namespace TheSSS.DICOMViewer.Security.DTOs;

/// <summary>
/// Carries license activation outcome (IsSuccess, Message, optional ValidatedLicenseInfo).
/// Requirement REQ-LDM-LIC-002, REQ-LDM-LIC-004.
/// </summary>
public record LicenseActivationResultDto(
    bool IsSuccess,
    string Message,
    string? ValidatedLicenseInfo = null // Could be a more complex object in a real scenario
);