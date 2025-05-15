using System;

namespace TheSSS.DICOMViewer.Integration.Configuration
{
    /// <summary>
    /// Configuration settings for interacting with the Odoo Licensing API.
    /// </summary>
    public class OdooApiSettings
    {
        /// <summary>
        /// The base URL of the Odoo API endpoint.
        /// Example: "https://odoo.example.com/api"
        /// </summary>
        public string BaseUrl { get; set; } = string.Empty;

        /// <summary>
        /// The API version to use (e.g., "v1"). This might be part of the BaseUrl or a separate path segment.
        /// </summary>
        public string ApiVersion { get; set; } = string.Empty;

        /// <summary>
        /// The relative path for the license validation endpoint.
        /// Example: "license/validate"
        /// </summary>
        public string ValidationEndpoint { get; set; } = string.Empty;

        /// <summary>
        /// The identifier used by the ICredentialManager to retrieve Odoo credentials.
        /// This key should match a configured entry in the credential store.
        /// Example: "OdooLicensingApi"
        /// </summary>
        public string ServiceIdentifierForCredentials { get; set; } = "OdooApi";

        /// <summary>
        /// Timeout for Odoo API calls.
        /// </summary>
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
    }
}