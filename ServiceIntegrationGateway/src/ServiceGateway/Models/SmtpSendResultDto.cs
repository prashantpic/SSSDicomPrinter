using System;

namespace TheSSS.DICOMViewer.Integration.Models
{
    /// <summary>
    /// Data Transfer Object representing the specific result from the SmtpServiceAdapter.
    /// This might contain more detailed information than EmailSendResultDto and is mapped by the coordinator.
    /// Corresponds to REQ-5-011.
    /// </summary>
    public record SmtpSendResultDto
    {
        /// <summary>
        /// Indicates if the SMTP operation was successful at the adapter level.
        /// </summary>
        public bool IsSuccess { get; init; }

        /// <summary>
        /// The message identifier returned by the SMTP server, if any.
        /// </summary>
        public string? MessageId { get; init; }

        /// <summary>
        /// Detailed status message from the SMTP client or server.
        /// Could include SMTP status codes or error descriptions.
        /// </summary>
        public string? DetailedStatus { get; init; }

        /// <summary>
        /// The SMTP status code, if an SmtpException occurred and was parsed.
        /// </summary>
        public System.Net.Mail.SmtpStatusCode? SmtpStatusCode { get; init; }


        public SmtpSendResultDto(bool isSuccess, string? messageId, string? detailedStatus, System.Net.Mail.SmtpStatusCode? smtpStatusCode = null)
        {
            IsSuccess = isSuccess;
            MessageId = messageId;
            DetailedStatus = detailedStatus;
            SmtpStatusCode = smtpStatusCode;
        }
    }
}