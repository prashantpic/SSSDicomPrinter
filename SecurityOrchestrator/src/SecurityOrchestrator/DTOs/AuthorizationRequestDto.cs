namespace TheSSS.DICOMViewer.Security.DTOs
{
    /// <summary>
    /// Data transfer object carrying details for an authorization check.
    /// REQ-7-005
    /// </summary>
    /// <param name="UserId">The unique identifier of the user whose authorization is being checked.</param>
    /// <param name="Permission">The permission being requested.</param>
    /// <param name="ResourceContext">Optional context for the resource being accessed (e.g., resource ID or type).</param>
    public record AuthorizationRequestDto(
        string UserId,
        string Permission,
        string? ResourceContext);
}