using System.Collections.Generic;

namespace TheSSS.DICOMViewer.Integration.Models
{
    /// <summary>
    /// Data Transfer Object for results of DICOM C-FIND operations.
    /// Typically containing a list of matched DICOM datasets.
    /// </summary>
    /// <remarks>
    /// `MatchedDatasets` contains a list of `object`. Each object represents a DICOM dataset
    /// returned by the C-FIND SCP. The actual type will depend on the `IDicomLowLevelClient` implementation.
    /// </remarks>
    public record DicomCFindResultDto(
        bool IsSuccess,
        ushort DicomStatusCode, // Final status of the C-FIND operation
        string StatusMessage,
        List<object>? MatchedDatasets, // List of matching DICOM datasets
        string? ErrorDetails = null
    );
}