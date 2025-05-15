namespace TheSSS.DICOMViewer.Integration.Configuration
{
    /// <summary>
    /// Root configuration class for the Service Integration Gateway.
    /// Aggregates specific settings for Odoo, SMTP, Print, DICOM, Resilience, Rate Limiting, and Credentials.
    /// </summary>
    public class ServiceGatewaySettings
    {
        public OdooApiSettings OdooApi { get; set; } = new();
        public SmtpSettings Smtp { get; set; } = new();
        public WindowsPrintSettings WindowsPrint { get; set; } = new();
        public DicomGatewaySettings DicomGateway { get; set; } = new();
        public ResilienceSettings Resilience { get; set; } = new();
        public RateLimitSettings RateLimiting { get; set; } = new();
        public CredentialManagerSettings Credentials { get; set; } = new();
    }
}