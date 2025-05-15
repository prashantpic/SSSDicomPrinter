using System.Collections.Generic;

namespace TheSSS.DICOMViewer.Security.DTOs
{
    /// <summary>
    /// Data transfer object representing the outcome of a user authentication attempt.
    /// REQ-7-006
    /// </summary>
    /// <param name="IsAuthenticated">Indicates whether the authentication was successful.</param>
    /// <param name="UserId">The unique identifier of the authenticated user, if successful.</param>
    /// <param name="UserName">The name of the authenticated user, if successful.</param>
    /// <param name="Roles">A collection of roles assigned to the authenticated user, if successful.</param>
    /// <param name="ErrorMessage">An error message if authentication failed.</param>
    public record AuthenticationResultDto(
        bool IsAuthenticated,
        string? UserId,
        string? UserName,
        IEnumerable<string>? Roles,
        string? ErrorMessage);
}