namespace TheSSS.DICOMViewer.Security.DTOs
{
    /// <summary>
    /// Data transfer object carrying credentials and type for a user authentication attempt.
    /// REQ-7-006
    /// </summary>
    /// <param name="Username">The username for authentication.</param>
    /// <param name="Password">The password for authentication.</param>
    /// <param name="AuthType">The type of authentication being attempted (e.g., "Local", "Windows").</param>
    public record AuthenticationRequestDto(
        string Username,
        string Password,
        string AuthType);
}