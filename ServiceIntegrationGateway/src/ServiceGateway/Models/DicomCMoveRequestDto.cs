namespace TheSSS.DICOMViewer.Integration.Models
{
    /// <summary>
    /// Data Transfer Object for DICOM C-MOVE requests via the gateway.
    /// Specifies destination AE, identifiers, and target AE.
    /// </summary>
    /// <remarks>
    /// The `IdentifiersDataset` is of type `object`. It is expected to be a representation
    /// of a DICOM dataset containing identifiers for the instances/series/studies to move,
    /// understandable by the underlying `IDicomLowLevelClient`.
    /// </remarks>
    public record DicomCMoveRequestDto(
        string TargetAE, // Source AE from which to move
        string TargetHost,
        int TargetPort,
        string DestinationAE, // AE Title of the destination
        object IdentifiersDataset, // DICOM dataset with identifiers for items to move
        string? CallingAET = null // Optional: Calling AETitle
    );
}