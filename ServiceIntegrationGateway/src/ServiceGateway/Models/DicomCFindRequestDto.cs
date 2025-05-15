using System.Collections.Generic;
// Using a generic Dictionary for queryDataset as specified in initial SDS (object QueryDataset).
// Alternatively, if FO-DICOM is a direct dependency, DicomDataset could be used.
// For loose coupling, Dictionary<string, string> or a custom TagValue pair list is safer.

namespace TheSSS.DICOMViewer.Integration.Models;

/// <summary>
/// Data Transfer Object for DICOM C-FIND requests via the gateway.
/// Specifies the target Application Entity (AE), query level, and query keys.
/// </summary>
/// <param name="TargetAe">The target DICOM Application Entity to send the C-FIND request to.</param>
/// <param name="QueryLevel">The hierarchical level of the query (e.g., "PATIENT", "STUDY", "SERIES", "IMAGE").
/// Should conform to DICOM standard values for QRLEVEL (0008,0052) if C-FIND-RQ.</param>
/// <param name="QueryDataset">A collection of DICOM tags and their values to be used as query criteria.
/// Represented as a dictionary where keys are DICOM tag strings (e.g., "0010,0010" for PatientName)
/// and values are the corresponding query values. Wildcards may be supported depending on the SCP.</param>
public record DicomCFindRequestDto(
    DicomAETarget TargetAe,
    string QueryLevel,
    Dictionary<string, string> QueryDataset // Using Dictionary as a representation of DICOM tags/values
                                            // This aligns with "object QueryDataset" intent for flexibility.
);