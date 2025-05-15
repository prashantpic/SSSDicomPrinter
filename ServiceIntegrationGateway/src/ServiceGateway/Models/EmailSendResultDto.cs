namespace TheSSS.DICOMViewer.Integration.Models
{
    /// <summary>
    /// Data Transfer Object representing the result of an email sending operation.
    /// </summary>
    public record EmailSendResultDto(
        bool IsSent,
        string? MessageId,
        string? StatusMessage
    );
}