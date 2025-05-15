namespace TheSSS.DICOMViewer.Security.DTOs
{
    /// <summary>
    /// Data transfer object representing the outcome of a license activation attempt.
    /// REQ-LDM-LIC-004
    /// </summary>
    /// <param name="IsSuccess">Indicates whether the license activation was successful.</param>
    /// <param name="Message">A message describing the outcome of the activation attempt.</param>
    /// <param name="ValidatedLicenseInfo">Validated license information if activation was successful and returned details.</param>
    public record LicenseActivationResultDto(
        bool IsSuccess,
        string Message,
        LicenseValidationResultDto? ValidatedLicenseInfo);
}