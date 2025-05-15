using System.Collections.Generic;

namespace TheSSS.DICOMViewer.Security.DTOs;

/// <summary>
/// Carries authentication outcome (IsAuthenticated, UserId, UserName, Roles, ErrorMessage).
/// Requirement REQ-7-006.
/// </summary>
public record AuthenticationResultDto(
    bool IsAuthenticated,
    string? UserId,
    string? UserName,
    IEnumerable<string>? Roles,
    string? ErrorMessage
);