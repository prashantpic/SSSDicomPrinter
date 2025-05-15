namespace TheSSS.DICOMViewer.Integration.Models
{
    /// <summary>
    /// Data Transfer Object representing the result of a print job submission.
    /// </summary>
    public record PrintResultDto(
        bool IsSubmitted,
        string? JobId,
        string? StatusMessage
    );
}