using System.Threading.Tasks;
using TheSSS.DICOMViewer.Security.DTOs;

namespace TheSSS.DICOMViewer.Security.Interfaces;

/// <summary>
/// Defines the contract for communication with the external Odoo licensing portal API
/// for license activation and validation.
/// Implemented in Infrastructure or ServiceIntegrationGateway.
/// Requirements Addressed: REQ-LDM-LIC-002, REQ-LDM-LIC-004.
/// </summary>
public interface ILicenseApiClient
{
    /// <summary>
    /// Validates the given license key and machine ID against the Odoo licensing portal.
    /// </summary>
    /// <param name="licenseKey">The license key to validate.</param>
    /// <param name="machineId">The unique machine identifier.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the license validation result.</returns>
    Task<LicenseValidationResultDto> ValidateLicenseAsync(string licenseKey, string machineId);

    /// <summary>
    /// Attempts to activate the given license key with the Odoo licensing portal for the specified machine ID.
    /// </summary>
    /// <param name="licenseKey">The license key to activate.</param>
    /// <param name="machineId">The unique machine identifier for which to activate the license.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the license activation result.</returns>
    Task<LicenseActivationResultDto> ActivateLicenseAsync(string licenseKey, string machineId);
}