namespace TheSSS.DICOMViewer.Integration.Configuration
{
    /// <summary>
    /// Configuration settings for SMTP email services.
    /// Includes server address, port, TLS/SSL requirements, authentication mechanisms, and credential identifiers.
    /// </summary>
    public class SmtpSettings
    {
        public string ServerAddress { get; set; } = string.Empty;
        public int Port { get; set; } = 25;
        public bool UseSsl { get; set; } = false;
        public bool UseTls { get; set; } = true; // Modern default
        public string? AuthenticationMechanism { get; set; } // e.g., "LOGIN", "PLAIN", null for default
        public string CredentialServiceIdentifier { get; set; } = "SmtpService"; // Identifier used with ICredentialManager
        public string DefaultFromAddress { get; set; } = string.Empty;
        public int TimeoutSeconds { get; set; } = 60;
    }
}