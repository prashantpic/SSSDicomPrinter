namespace TheSSS.DICOMViewer.Integration.Configuration;

/// <summary>
/// Configuration settings for SMTP email services.
/// Includes server address, port, TLS/SSL requirements, authentication mechanisms, and credential identifiers.
/// </summary>
public class SmtpSettings
{
    /// <summary>
    /// Gets or sets the SMTP server address.
    /// </summary>
    public string Server { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the SMTP port.
    /// </summary>
    public int Port { get; set; } = 587; // Default SMTP port for TLS

    /// <summary>
    /// Gets or sets a value indicating whether to use SSL/TLS.
    /// </summary>
    public bool EnableSsl { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether SMTP authentication is required.
    /// If true, Username and Password or equivalent credentials must be provided via ICredentialManager.
    /// </summary>
    public bool RequiresAuthentication { get; set; } = true;

    /// <summary>
    /// Gets or sets the key used to look up credentials in ICredentialManager (e.g., "SmtpService").
    /// </summary>
    public string ServiceIdentifierForCredentials { get; set; } = "SmtpService";

    /// <summary>
    /// Gets or sets the key for retrieving the specific resilience policy for SMTP operations from IResiliencePolicyProvider.
    /// </summary>
    public string PolicyKey { get; set; } = "SmtpPolicy";
}