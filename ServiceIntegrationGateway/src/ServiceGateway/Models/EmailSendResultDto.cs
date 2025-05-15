namespace TheSSS.DICOMViewer.Integration.Models;

/// <summary>
/// Data Transfer Object representing the result of an email sending operation as exposed by the gateway.
/// </summary>
/// <param name="IsSentSuccessfully">Indicates whether the email was successfully sent or queued.</param>
/// <param name="Message">A message providing details about the sending status (e.g., "Email sent successfully", "Failed to connect to SMTP server").</param>
/// <param name="MessageId">An optional message identifier returned by the SMTP service, if available.</param>
public record EmailSendResultDto(
    bool IsSentSuccessfully,
    string Message,
    string? MessageId
);