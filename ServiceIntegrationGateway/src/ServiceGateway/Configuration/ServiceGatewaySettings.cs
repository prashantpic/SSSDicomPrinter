using System.Collections.Generic;

namespace TheSSS.DICOMViewer.Integration.Configuration;

/// <summary>
/// Root configuration class for the Service Integration Gateway.
/// Aggregates specific settings for Odoo, SMTP, Print, DICOM, Resilience, Rate Limiting, and Credentials.
/// </summary>
public class ServiceGatewaySettings
{
    /// <summary>
    /// Gets or sets the configuration settings for the Odoo API integration.
    /// </summary>
    public OdooApiSettings OdooApi { get; set; } = new();

    /// <summary>
    /// Gets or sets the configuration settings for the SMTP service integration.
    /// </summary>
    public SmtpSettings Smtp { get; set; } = new();

    /// <summary>
    /// Gets or sets the configuration settings for the Windows Print API integration.
    /// </summary>
    public WindowsPrintSettings WindowsPrint { get; set; } = new();

    /// <summary>
    /// Gets or sets the configuration settings for the DICOM network integration.
    /// </summary>
    public DicomGatewaySettings DicomGateway { get; set; } = new();

    /// <summary>
    /// Gets or sets the configuration settings for resilience policies (e.g., retry, circuit breaker).
    /// </summary>
    public ResilienceSettings Resilience { get; set; } = new();

    /// <summary>
    /// Gets or sets the configuration settings for rate limiting.
    /// </summary>
    public RateLimitSettings RateLimiting { get; set; } = new();

    /// <summary>
    /// Gets or sets the configuration settings for the credential manager.
    /// </summary>
    public CredentialManagerSettings CredentialManager { get; set; } = new();

    /// <summary>
    /// Gets or sets a value indicating whether Odoo integration is enabled.
    /// </summary>
    public bool EnableOdooIntegration { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether SMTP integration is enabled.
    /// </summary>
    public bool EnableSmtpIntegration { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether Windows Print integration is enabled.
    /// </summary>
    public bool EnablePrintIntegration { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether DICOM network integration is enabled.
    /// </summary>
    public bool EnableDicomIntegration { get; set; } = false;
}