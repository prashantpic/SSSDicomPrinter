using System;
using System.Security.Authentication; // For SslProtocols

namespace TheSSS.DICOMViewer.Integration.Configuration
{
    /// <summary>
    /// Configuration settings for SMTP email services.
    /// </summary>
    public class SmtpSettings
    {
        /// <summary>
        /// The address of the SMTP server.
        /// Example: "smtp.example.com"
        /// </summary>
        public string Server { get; set; } = string.Empty;

        /// <summary>
        /// The port number for the SMTP server.
        /// Common ports: 25 (SMTP), 587 (SMTP with STARTTLS), 465 (SMTPS - SSL/TLS).
        /// </summary>
        public int Port { get; set; } = 587;

        /// <summary>
        /// Specifies if SSL/TLS should be enabled for the connection.
        /// For port 465, this is typically true. For port 587 (STARTTLS), SmtpClient handles it.
        /// </summary>
        public bool EnableSsl { get; set; } = true; // Often true for modern SMTP servers

        /// <summary>
        /// Specifies the SSL/TLS protocols to be used.
        /// It's recommended to use system defaults or modern protocols like Tls12, Tls13.
        /// Note: System.Net.Mail.SmtpClient has limited direct control over this.
        /// Libraries like MailKit offer more fine-grained control.
        /// </summary>
        public SslProtocols SslProtocols { get; set; } = SslProtocols.Tls12 | SslProtocols.Tls13;

        /// <summary>
        /// Specifies if authentication is required to send emails.
        /// </summary>
        public bool RequiresAuthentication { get; set; } = true;

        /// <summary>
        /// The identifier used by the ICredentialManager to retrieve SMTP credentials.
        /// Example: "SmtpServiceCredentials"
        /// </summary>
        public string ServiceIdentifierForCredentials { get; set; } = "SmtpService";

        /// <summary>
        /// Default sender email address.
        /// </summary>
        public string DefaultFromAddress { get; set; } = string.Empty;

        /// <summary>
        /// Default sender display name.
        /// </summary>
        public string DefaultFromDisplayName { get; set; } = string.Empty;

        /// <summary>
        /// Timeout for SMTP operations.
        /// </summary>
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(60);
    }
}