using System.Threading.Tasks;
using TheSSS.DICOMViewer.Security.DTOs;

namespace TheSSS.DICOMViewer.Security.Services
{
    /// <summary>
    /// Defines the contract for orchestrating all license-related operations,
    /// including activation, startup validation, and periodic checks.
    /// REQ-LDM-LIC-002, REQ-LDM-LIC-005
    /// </summary>
    public interface ILicenseOrchestrationService
    {
        /// <summary>
        /// Validates the application license on startup.
        /// </summary>
        /// <returns>A <see cref="LicenseValidationResultDto"/> indicating the outcome of the validation.</returns>
        Task<LicenseValidationResultDto> ValidateLicenseOnStartupAsync();

        /// <summary>
        /// Performs a periodic license check.
        /// </summary>
        /// <returns>A <see cref="LicenseValidationResultDto"/> indicating the outcome of the periodic check.</returns>
        Task<LicenseValidationResultDto> PerformPeriodicLicenseCheckAsync();

        /// <summary>
        /// Activates the application license using the provided license key.
        /// </summary>
        /// <param name="licenseKey">The license key to activate.</param>
        /// <returns>A <see cref="LicenseActivationResultDto"/> indicating the outcome of the activation attempt.</returns>
        Task<LicenseActivationResultDto> ActivateLicenseAsync(string licenseKey);
    }
}