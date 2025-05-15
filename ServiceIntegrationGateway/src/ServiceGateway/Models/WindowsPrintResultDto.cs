namespace TheSSS.DICOMViewer.Integration.Models
{
    /// <summary>
    /// Data Transfer Object representing the specific result from the WindowsPrintAdapter.
    /// Encapsulates the success/failure status and any relevant details from a Windows Print API operation.
    /// </summary>
    public record WindowsPrintResultDto(
        bool IsSubmitted,
        string? JobId, // Native Print Job ID, if available
        string? StatusMessage // Status or error message from the print system
    );
}