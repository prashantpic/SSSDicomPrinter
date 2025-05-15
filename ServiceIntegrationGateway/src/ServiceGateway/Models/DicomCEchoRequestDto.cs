namespace TheSSS.DICOMViewer.Integration.Models;

/// <summary>
/// Data Transfer Object for DICOM C-ECHO requests via the gateway.
/// Specifies the target Application Entity (AE) details for the verification.
/// </summary>
/// <param name="TargetAe">The target DICOM Application Entity to send the C-ECHO request to.</param>
public record DicomCEchoRequestDto(
    DicomAETarget TargetAe
);