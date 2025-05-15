using System.Collections.Generic;

namespace TheSSS.DICOMViewer.Integration.Models;

/// <summary>
/// Data Transfer Object for generic results of DICOM operations like C-STORE, C-ECHO, C-MOVE.
/// Indicates success/failure and includes messages or status codes.
/// </summary>
/// <param name="IsSuccessful">Indicates whether the DICOM operation was broadly successful.
/// For operations involving multiple sub-operations (like C-STORE of many files), this might be true if any part succeeded,
/// or only if all parts succeeded, depending on implementation. Check DicomStatusCode for specifics.</param>
/// <param name="DicomStatusCode">The primary DICOM status code returned by the operation (e.g., 0x0000 for Success).
/// Refer to DICOM standard Part 7, Annex C for status code meanings.</param>
/// <param name="StatusMessage">A human-readable message describing the outcome, potentially derived from the DicomStatusCode or additional error info.</param>
/// <param name="AffectedSopInstanceUids">An optional list of SOP Instance UIDs that were affected by the operation (e.g., successfully stored or moved).
/// This can be useful for C-STORE or C-MOVE to track individual instance statuses.</param>
public record DicomOperationResultDto(
    bool IsSuccessful,
    ushort DicomStatusCode,
    string StatusMessage,
    List<string>? AffectedSopInstanceUids = null
);