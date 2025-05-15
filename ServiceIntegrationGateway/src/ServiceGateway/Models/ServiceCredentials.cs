using System;
using System.Collections.Generic;

namespace TheSSS.DICOMViewer.Integration.Models
{
    /// <summary>
    /// Represents credentials for an external service. This is a flexible class
    /// to accommodate various authentication schemes (username/password, API key, token, etc.).
    /// </summary>
    public class ServiceCredentials
    {
        /// <summary>
        /// Username for basic authentication or similar schemes.
        /// </summary>
        public string? Username { get; set; }

        /// <summary>
        /// Password for basic authentication or similar schemes.
        /// Should be handled securely (e.g., as SecureString internally if possible, though string is common for DTOs).
        /// </summary>
        public string? Password { get; set; }

        /// <summary>
        /// API key for services that use API key authentication.
        /// </summary>
        public string? ApiKey { get; set; }

        /// <summary>
        /// Bearer token, OAuth token, or other types of authentication tokens.
        /// </summary>
        public string? Token { get; set; }

        /// <summary>
        /// For more complex or custom authentication schemes, additional properties can be stored here.
        /// For example, client ID/secret for OAuth, or specific header names/values.
        /// </summary>
        public Dictionary<string, string> AdditionalProperties { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Optional: Expiration time for the token or credentials, if known.
        /// This can help the CredentialManager decide if cached credentials are still valid.
        /// </summary>
        public DateTime? ExpiresAtUtc { get; set; }

        public ServiceCredentials() { }

        /// <summary>
        /// Checks if any credential part is set.
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        public bool IsEmpty =>
            string.IsNullOrWhiteSpace(Username) &&
            string.IsNullOrWhiteSpace(Password) &&
            string.IsNullOrWhiteSpace(ApiKey) &&
            string.IsNullOrWhiteSpace(Token) &&
            (AdditionalProperties == null || AdditionalProperties.Count == 0);
    }
}