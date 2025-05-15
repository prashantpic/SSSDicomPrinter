using System;

namespace TheSSS.DICOMViewer.Integration.Models
{
    /// <summary>
    /// Data Transfer Object representing the specific result from the WindowsPrintAdapter.
    /// This might contain more detailed information than PrintResultDto and is mapped by the coordinator.
    /// Corresponds to REQ-5-001.
    /// </summary>
    public record WindowsPrintResultDto
    {
        /// <summary>
        /// Indicates if the print operation was successful at the adapter level.
        /// </summary>
        public bool IsSuccess { get; init; }

        /// <summary>
        /// The job identifier returned by the Windows Print API, if any.
        /// </summary>
        public string? JobId { get; init; } // Can be string or int depending on API

        /// <summary>
        /// Detailed status message from the Windows Print API interaction.
        /// Could include error codes or descriptions.
        /// </summary>
        public string? DetailedStatus { get; init; }

        /// <summary>
        /// Native error code from the print system, if an error occurred.
        /// </summary>
        public int? NativeErrorCode { get; init; }

        public WindowsPrintResultDto(bool isSuccess, string? jobId, string? detailedStatus, int? nativeErrorCode = null)
        {
            IsSuccess = isSuccess;
            JobId = jobId;
            DetailedStatus = detailedStatus;
            NativeErrorCode = nativeErrorCode;
        }
    }
}