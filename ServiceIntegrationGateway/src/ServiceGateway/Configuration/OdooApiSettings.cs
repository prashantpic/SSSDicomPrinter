namespace TheSSS.DICOMViewer.Integration.Configuration
{
    /// <summary>
    /// Configuration settings for interacting with the Odoo Licensing API.
    /// Includes Base URL, API version, specific endpoint paths for activation/validation, and credential identifiers.
    /// </summary>
    public class OdooApiSettings
    {
        public string BaseUrl { get; set; } = string.Empty;
        public string ApiVersion { get; set; } = "v1";
        public string ValidationEndpoint { get; set; } = "/api/license/validate";
        public string ActivationEndpoint { get; set; } = "/api/license/activate";
        public string CredentialServiceIdentifier { get; set; } = "OdooApi"; // Identifier used with ICredentialManager
        public int TimeoutSeconds { get; set; } = 30;
    }
}