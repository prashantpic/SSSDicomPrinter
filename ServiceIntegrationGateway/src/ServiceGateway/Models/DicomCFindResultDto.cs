using System.Collections.Generic;
// Using Dictionary<string, string> to represent a DICOM dataset for loose coupling,
// aligning with DicomCFindRequestDto.QueryDataset and initial SDS for "object MatchedDatasets".

namespace TheSSS.DICOMViewer.Integration.Models;

/// <summary>
/// Data Transfer Object for results of DICOM C-FIND operations.
/// Contains a list of matched DICOM datasets and the overall operation status.
/// </summary>
/// <param name="IsSuccessful">Indicates whether the C-FIND operation itself completed without critical errors.
/// Note that a successful C-FIND operation might still yield zero matches.</param>
/// <param name="DicomStatusCode">The DICOM status code of the C-FIND operation (e.g., Success, Pending, Failure).</param>
/// <param name="StatusMessage">A human-readable message describing the outcome of the C-FIND operation.</param>
/// <param name="MatchedDatasets">A list of datasets that matched the query criteria. Each dataset is represented as a
/// dictionary where keys are DICOM tag strings (e.g., "0010,0010") and values are the corresponding values.
/// This list can be empty if no matches were found but the operation was otherwise successful.</param>
public record DicomCFindResultDto(
    bool IsSuccessful,
    ushort DicomStatusCode,
    string StatusMessage,
    List<Dictionary<string, string>>? MatchedDatasets // List of simplified DICOM datasets
);