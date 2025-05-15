using System;

namespace TheSSS.DICOMViewer.Integration.Models
{
    /// <summary>
    /// Data Transfer Object for Odoo license validation requests, used by OdooApiAdapter.
    /// Corresponds to REQ-LDM-LIC-004.
    /// </summary>
    public record OdooLicenseRequestDto
    {
        /// <summary>
        /// The license key to be validated.
        /// </summary>
        public string LicenseKey { get; init; }

        /// <summary>
        /// Optional: A unique identifier for the machine or instance requesting validation.
        /// This can help Odoo track activations.
        /// </summary>
        public string? MachineId { get; init; }

        /// <summary>
        /// Optional: The version of the application making the request.
        /// </summary>
        public string? ApplicationVersion { get; init; }

        // Constructor allowing only license key
        public OdooLicenseRequestDto(string licenseKey)
        {
            if (string.IsNullOrWhiteSpace(licenseKey))
                throw new ArgumentException("License key cannot be null or whitespace.", nameof(licenseKey));
            LicenseKey = licenseKey;
        }

        // Constructor for all properties
        public OdooLicenseRequestDto(string licenseKey, string? machineId, string? applicationVersion)
            : this(licenseKey)
        {
            MachineId = machineId;
            ApplicationVersion = applicationVersion;
        }
    }
}