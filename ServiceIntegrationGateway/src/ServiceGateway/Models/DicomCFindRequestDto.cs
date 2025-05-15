namespace TheSSS.DICOMViewer.Integration.Models
{
    /// <summary>
    /// Data Transfer Object for DICOM C-FIND requests via the gateway.
    /// Specifies query level, keys, and target AE.
    /// </summary>
    /// <remarks>
    /// The `QueryDataset` is of type `object`. It is expected to be a representation
    /// of a DICOM dataset understandable by the underlying `IDicomLowLevelClient`.
    /// This could be a `DicomDataset` from a library like fo-dicom, or a custom simplified structure.
    /// </remarks>
    public record DicomCFindRequestDto(
        string TargetAE,
        string TargetHost,
        int TargetPort,
        string QueryLevel, // e.g., "PATIENT", "STUDY", "SERIES", "IMAGE"
        object QueryDataset, // The DICOM dataset containing query keys
        string? CallingAET = null // Optional: Calling AETitle
    );
}