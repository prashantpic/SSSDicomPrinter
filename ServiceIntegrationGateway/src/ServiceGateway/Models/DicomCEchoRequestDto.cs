namespace TheSSS.DICOMViewer.Integration.Models
{
    /// <summary>
    /// Data Transfer Object for DICOM C-ECHO requests via the gateway.
    /// Specifies the target AE details.
    /// </summary>
    public record DicomCEchoRequestDto(
        string TargetAE,
        string TargetHost,
        int TargetPort,
        string? CallingAET = null // Optional: Calling AETitle
    );
}