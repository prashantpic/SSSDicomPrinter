namespace TheSSS.DICOMViewer.Security.DTOs
{
    /// <summary>
    /// Data transfer object carrying information required for validating a software license.
    /// REQ-LDM-LIC-002, REQ-LDM-LIC-004
    /// </summary>
    /// <param name="LicenseKey">The license key to validate.</param>
    /// <param name="MachineIdentifier">The unique identifier of the machine requesting validation.</param>
    public record LicenseValidationRequestDto(
        string LicenseKey,
        string MachineIdentifier);
}