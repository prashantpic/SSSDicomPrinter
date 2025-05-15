using System.Security.Cryptography;

namespace TheSSS.DICOMViewer.Security.Configuration;

/// <summary>
/// Holds configurable settings for the SecurityOrchestrator module.
/// These settings are typically loaded from application configuration (e.g., appsettings.json).
/// Sensitive values like API client IDs/secrets are expected to be loaded via secure configuration providers
/// and are not directly part of this class structure.
/// Requirements Addressed: REQ-LDM-LIC-002, REQ-7-017, REQ-LDM-LIC-004.
/// </summary>
public class SecurityOrchestratorSettings
{
    /// <summary>
    /// Gets or sets the interval in hours for periodic license checks.
    /// Default: 24 hours.
    /// Requirement: REQ-LDM-LIC-002.
    /// </summary>
    public int LicenseCheckIntervalHours { get; set; } = 24;

    /// <summary>
    /// Gets or sets the base URL for the Odoo licensing API.
    /// Requirement: REQ-LDM-LIC-004.
    /// </summary>
    public string? OdooApiBaseUrl { get; set; }

    /// <summary>
    /// Gets or sets the default scope for DPAPI (Data Protection API) operations.
    /// Default: CurrentUser.
    /// Requirement: REQ-7-017.
    /// </summary>
    public DataProtectionScope DefaultDataProtectionScope { get; set; } = DataProtectionScope.CurrentUser;

    /// <summary>
    /// Gets or sets the source or method for determining the unique machine ID
    /// used for license validation.
    /// Default: "Default" (implementation specific interpretation).
    /// Requirement: REQ-LDM-LIC-002.
    /// </summary>
    public string MachineIdentifierSource { get; set; } = "Default";

    // Note: Additional settings like OdooApiClientId, OdooApiClientSecret (REQ-LDM-LIC-004)
    // are expected to be loaded via secure configuration providers (e.g., environment variables, Azure Key Vault)
    // and accessed by the ILicenseApiClient implementation, not directly stored here.

    // Authentication Mode (e.g., "Authentication:Mode") is typically read by the
    // IIdentityProviderService factory or DI setup to select the correct provider,
    // not directly consumed by the orchestrator services themselves from this settings object.
}