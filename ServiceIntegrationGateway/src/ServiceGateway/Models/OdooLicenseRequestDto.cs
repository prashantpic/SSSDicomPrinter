namespace TheSSS.DICOMViewer.Integration.Models;

/// <summary>
/// Data Transfer Object for Odoo license validation requests, used by the OdooApiAdapter.
/// Represents the data structure sent to the Odoo API for license validation.
/// </summary>
/// <param name="LicenseKey">The license key to be validated.</param>
/// <param name="MachineIdentifier">An optional machine identifier, if required by the Odoo API for validation.</param>
public record OdooLicenseRequestDto(
    string LicenseKey,
    string? MachineIdentifier = null // Made optional as per initial SDS, can be added if needed
);