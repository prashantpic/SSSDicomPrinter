namespace TheSSS.DICOMViewer.Integration.Configuration;

/// <summary>
/// Configuration settings for interacting with the Odoo Licensing API.
/// Includes Base URL, API version, specific endpoint paths for activation/validation, and credential identifiers.
/// </summary>
public class OdooApiSettings
{
    /// <summary>
    /// Gets or sets the base URL for the Odoo API.
    /// </summary>
    public string BaseUrl { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the API version to use (e.g., "1").
    /// </summary>
    public string ApiVersion { get; set; } = "1";

    /// <summary>
    /// Gets or sets the specific endpoint path format for license validation (e.g., "/api/v{0}/license/validate").
    /// {0} will be replaced with ApiVersion.
    /// </summary>
    public string LicenseValidationEndpoint { get; set; } = "/api/v{0}/license/validate";

    /// <summary>
    /// Gets or sets the key used to look up credentials in ICredentialManager (e.g., "OdooApi").
    /// </summary>
    public string ServiceIdentifierForCredentials { get; set; } = "OdooApi";

    /// <summary>
    /// Gets or sets the key for retrieving the specific resilience policy for Odoo API calls from IResiliencePolicyProvider.
    /// </summary>
    public string PolicyKey { get; set; } = "OdooApiPolicy";

    /// <summary>
    /// Gets or sets the resource key for rate limiting Odoo API calls, used by IRateLimiter.
    /// </summary>
    public string RateLimitResourceKey { get; set; } = "OdooApi";
}