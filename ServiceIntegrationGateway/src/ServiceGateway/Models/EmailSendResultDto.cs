using System;

namespace TheSSS.DICOMViewer.Integration.Models
{
    /// <summary>
    /// Data Transfer Object representing the result of an email sending operation as exposed by the gateway.
    /// Corresponds to REQ-5-011.
    /// </summary>
    public record EmailSendResultDto
    {
        /// <summary>
        /// Indicates whether the email was successfully accepted by the SMTP server for delivery.
        /// </summary>
        public bool Success { get; init; }

        /// <summary>
        /// A unique message identifier assigned by the SMTP server, if available.
        /// </summary>
        public string? MessageId { get; init; }

        /// <summary>
        /// A status message from the SMTP operation, providing more context on success or failure.
        /// </summary>
        public string? StatusMessage { get; init; }

        public EmailSendResultDto(bool success, string? messageId, string? statusMessage)
        {
            Success = success;
            MessageId = messageId;
            StatusMessage = statusMessage;
        }
    }
}