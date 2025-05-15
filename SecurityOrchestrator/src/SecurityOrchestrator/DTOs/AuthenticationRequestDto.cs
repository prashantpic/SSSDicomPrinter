namespace TheSSS.DICOMViewer.Security.DTOs;

/// <summary>
/// Carries username, password, and authentication type for authentication requests.
/// Requirement REQ-7-006.
/// </summary>
public record AuthenticationRequestDto(
    string Username,
    string Password,
    string AuthType
);