namespace TheSSS.DICOMViewer.Integration.Models;

/// <summary>
/// Data Transfer Object representing the specific result from the SmtpServiceAdapter.
/// This DTO is returned by the adapter and may be mapped by the coordinator to a more generic EmailSendResultDto.
/// </summary>
/// <param name="IsSentSuccessfully">Indicates whether the SMTP operation reported success.</param>
/// <param name="StatusMessage">A descriptive message from the SMTP adapter regarding the operation's outcome.</param>
/// <param name="SmtpStatusCode">An optional SMTP status code returned by the server, if applicable and retrievable.</param>
/// <param name="MessageId">An optional unique identifier for the sent message, if provided by the SMTP server or library.</param>
public record SmtpSendResultDto(
    bool IsSentSuccessfully,
    string StatusMessage,
    int? SmtpStatusCode, // Example: System.Net.Mail.SmtpStatusCode can be mapped here
    string? MessageId
);