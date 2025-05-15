namespace TheSSS.DICOMViewer.Integration.Models
{
    /// <summary>
    /// Data Transfer Object representing the specific result from the SmtpServiceAdapter.
    /// Encapsulates the success/failure status and any relevant details from an SMTP send operation.
    /// </summary>
    public record SmtpSendResultDto(
        bool IsSent,
        string? MessageId, // Optional: SMTP server might return an ID for the sent message
        string? StatusMessage // Any additional status information or error message from SMTP server
    );
}