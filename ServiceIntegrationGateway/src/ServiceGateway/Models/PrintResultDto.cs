using System;

namespace TheSSS.DICOMViewer.Integration.Models
{
    /// <summary>
    /// Data Transfer Object representing the result of a print job submission as exposed by the gateway.
    /// Corresponds to REQ-5-001.
    /// </summary>
    public record PrintResultDto
    {
        /// <summary>
        /// Indicates whether the print job was successfully submitted to the print spooler.
        /// </summary>
        public bool Success { get; init; }

        /// <summary>
        /// The job identifier assigned by the print system, if available.
        /// </summary>
        public string? JobId { get; init; } // Changed to string as Job IDs can be non-integer

        /// <summary>
        /// A status message from the print operation.
        /// </summary>
        public string? StatusMessage { get; init; }

        public PrintResultDto(bool success, string? jobId, string? statusMessage)
        {
            Success = success;
            JobId = jobId;
            StatusMessage = statusMessage;
        }
    }
}