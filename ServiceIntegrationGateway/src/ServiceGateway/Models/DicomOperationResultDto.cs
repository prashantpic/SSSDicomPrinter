using System.Collections.Generic;

namespace TheSSS.DICOMViewer.Integration.Models
{
    /// <summary>
    /// Data Transfer Object for generic results of DICOM operations (C-STORE, C-ECHO, C-MOVE).
    /// Indicates success/failure and any messages or status codes.
    /// </summary>
    public record DicomOperationResultDto(
        bool IsSuccess,
        ushort DicomStatusCode, // Standard DICOM status code (0x0000 for success)
        string StatusMessage, // Human-readable message corresponding to the status code or error
        List<string>? AffectedSopInstanceUids, // For C-STORE, list of SOP Instance UIDs successfully stored or failed
        string? ErrorDetails = null // Additional error details if any
    );
}