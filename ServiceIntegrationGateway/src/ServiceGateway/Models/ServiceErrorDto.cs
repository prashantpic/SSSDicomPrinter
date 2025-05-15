using System;

namespace TheSSS.DICOMViewer.Integration.Models
{
    /// <summary>
    /// Common Data Transfer Object for representing errors from any service integration,
    /// providing a standardized error structure.
    /// </summary>
    public record ServiceErrorDto
    {
        /// <summary>
        /// A unique, machine-readable error code (e.g., "ODOO_API_ERROR_401", "SMTP_AUTH_FAILED").
        /// </summary>
        public string ErrorCode { get; init; }

        /// <summary>
        /// A human-readable error message suitable for display to users or for logging.
        /// </summary>
        public string Message { get; init; }

        /// <summary>
        /// Optional: Additional technical details about the error, which might include
        /// a stack trace snippet (in debug/dev environments), or specific data points that caused the error.
        /// Should be used carefully to avoid exposing sensitive information.
        /// </summary>
        public string? Details { get; init; }

        /// <summary>
        /// Identifier of the source service where the error originated (e.g., "OdooApiAdapter", "SmtpService", "DicomNetwork").
        /// </summary>
        public string SourceService { get; init; }

        /// <summary>
        /// Optional: Timestamp when the error occurred.
        /// </summary>
        public DateTime TimestampUtc { get; init; }

        public ServiceErrorDto(string errorCode, string message, string sourceService, string? details = null)
        {
            if (string.IsNullOrWhiteSpace(errorCode))
                throw new ArgumentException("Error code cannot be null or whitespace.", nameof(errorCode));
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("Message cannot be null or whitespace.", nameof(message));
            if (string.IsNullOrWhiteSpace(sourceService))
                throw new ArgumentException("Source service cannot be null or whitespace.", nameof(sourceService));

            ErrorCode = errorCode;
            Message = message;
            Details = details;
            SourceService = sourceService;
            TimestampUtc = DateTime.UtcNow;
        }
    }
}