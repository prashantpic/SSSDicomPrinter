using System;
using System.Collections.Generic;
using Dicom; // From fo-dicom-core

namespace TheSSS.DICOMViewer.Integration.Models
{
    /// <summary>
    /// Data Transfer Object for results of DICOM C-FIND operations.
    /// </summary>
    public record DicomCFindResultDto
    {
        /// <summary>
        /// Indicates whether the C-FIND operation completed successfully (e.g., status Success or Pending).
        /// Note: A "successful" C-FIND might still yield zero results.
        /// </summary>
        public bool Success { get; init; }

        /// <summary>
        /// The final DICOM status code for the C-FIND operation.
        /// (e.g., 0x0000 for Success, 0xFF00 or 0xFF01 for Pending, others for failure/cancel).
        /// </summary>
        public ushort DicomStatusCode { get; init; }

        /// <summary>
        /// A human-readable status message.
        /// </summary>
        public string? StatusMessage { get; init; }

        /// <summary>
        /// A list of DicomDataset objects representing the matched results from the C-FIND query.
        /// This list accumulates results from C-FIND-RSP messages where status is Pending.
        /// </summary>
        public List<DicomDataset> MatchedDatasets { get; init; } = new List<DicomDataset>();

        public DicomCFindResultDto(bool success, ushort dicomStatusCode, string? statusMessage, List<DicomDataset>? matchedDatasets = null)
        {
            Success = success;
            DicomStatusCode = dicomStatusCode;
            StatusMessage = statusMessage;
            MatchedDatasets = matchedDatasets ?? new List<DicomDataset>();
        }
    }
}