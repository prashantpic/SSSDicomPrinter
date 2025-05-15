using System;
using System.Collections.Generic;

namespace TheSSS.DICOMViewer.Integration.Models
{
    /// <summary>
    /// Data Transfer Object for generic results of DICOM operations like C-STORE, C-ECHO, C-MOVE.
    /// </summary>
    public record DicomOperationResultDto
    {
        /// <summary>
        /// Indicates whether the DICOM operation was successful overall.
        /// For C-STORE, this might mean all instances were successfully sent.
        /// </summary>
        public bool Success { get; init; }

        /// <summary>
        /// The final DICOM status code for the operation. (e.g., 0x0000 for Success).
        /// </summary>
        public ushort DicomStatusCode { get; init; }

        /// <summary>
        /// A human-readable status message, often corresponding to the DicomStatusCode.
        /// </summary>
        public string? StatusMessage { get; init; }

        /// <summary>
        /// For C-STORE, a list of SOP Instance UIDs that were successfully transferred.
        /// For other operations, this might be null or empty.
        /// </summary>
        public List<string>? AffectedSopInstanceUids { get; init; }

        /// <summary>
        /// Optional: Number of remaining sub-operations (e.g., for C-MOVE).
        /// </summary>
        public int? RemainingSubOperations { get; init; }

        /// <summary>
        /// Optional: Number of completed sub-operations.
        /// </summary>
        public int? CompletedSubOperations { get; init; }

        /// <summary>
        /// Optional: Number of sub-operations with warnings.
        /// </summary>
        public int? WarningSubOperations { get; init; }

        /// <summary>
        /// Optional: Number of failed sub-operations.
        /// </summary>
        public int? FailedSubOperations { get; init; }


        public DicomOperationResultDto(
            bool success,
            ushort dicomStatusCode,
            string? statusMessage,
            List<string>? affectedSopInstanceUids = null,
            int? remaining = null,
            int? completed = null,
            int? warnings = null,
            int? failed = null
            )
        {
            Success = success;
            DicomStatusCode = dicomStatusCode;
            StatusMessage = statusMessage;
            AffectedSopInstanceUids = affectedSopInstanceUids;
            RemainingSubOperations = remaining;
            CompletedSubOperations = completed;
            WarningSubOperations = warnings;
            FailedSubOperations = failed;
        }
    }
}