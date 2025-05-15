namespace TheSSS.DICOMViewer.Integration.Models
{
    /// <summary>
    /// Data Transfer Object for Odoo license validation requests, used by OdooApiAdapter.
    /// Represents the data structure sent to the Odoo API for license validation.
    /// </summary>
    public record OdooLicenseRequestDto(
        string LicenseKey,
        string DeviceIdentifier // Could be MAC address, system UUID, etc.
    );
}