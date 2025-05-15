using System;

namespace TheSSS.DICOMViewer.Integration.Configuration
{
    /// <summary>
    /// Root configuration class for the Service Integration Gateway, aggregating specific settings
    /// for Odoo, SMTP, Print, DICOM, Resilience, Rate Limiting, and Credentials.
    /// </summary>
    public class ServiceGatewaySettings
    {
        /// <summary>
        /// Settings specific to the Odoo Licensing API integration.
        /// </summary>
        public OdooApiSettings OdooApi { get; set; } = new OdooApiSettings();

        /// <summary>
        /// Settings specific to the SMTP email service integration.
        /// </summary>
        public SmtpSettings Smtp { get; set; } = new SmtpSettings();

        /// <summary>
        /// Settings specific to the Windows Print API integration.
        /// </summary>
        public WindowsPrintSettings WindowsPrint { get; set; } = new WindowsPrintSettings();

        /// <summary>
        /// Settings specific to the DICOM network communications integration.
        /// </summary>
        public DicomGatewaySettings Dicom { get; set; } = new DicomGatewaySettings();

        /// <summary>
        /// Settings for resilience policies (Circuit Breaker, Retry, Timeout).
        /// </summary>
        public ResilienceSettings Resilience { get; set; } = new ResilienceSettings();

        /// <summary>
        /// Settings for API rate limiting policies.
        /// </summary>
        public RateLimitSettings RateLimiting { get; set; } = new RateLimitSettings();

        /// <summary>
        /// Settings for the credential management service.
        /// </summary>
        public CredentialManagerSettings CredentialManager { get; set; } = new CredentialManagerSettings();
    }
}