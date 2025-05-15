namespace TheSSS.DICOMViewer.Integration.Models
{
    /// <summary>
    /// Common Data Transfer Object for representing errors from any service integration.
    /// Provides a standardized error structure.
    /// </summary>
    public record ServiceErrorDto(
        string Code,       // A service-specific or gateway-defined error code
        string Message,    // A human-readable error message
        string? Details,    // Detailed information about the error, stack trace, etc.
        string? SourceService // Identifier of the service that originated the error
    );
}