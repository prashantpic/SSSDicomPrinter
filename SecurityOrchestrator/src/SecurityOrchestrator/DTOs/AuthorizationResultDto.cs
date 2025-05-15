namespace TheSSS.DICOMViewer.Security.DTOs
{
    /// <summary>
    /// Data transfer object representing the outcome of an authorization check.
    /// REQ-7-005
    /// </summary>
    /// <param name="IsAuthorized">Indicates whether the user is authorized for the requested permission.</param>
    /// <param name="Reason">A reason for the authorization outcome, especially if not authorized.</param>
    public record AuthorizationResultDto(
        bool IsAuthorized,
        string? Reason);
}