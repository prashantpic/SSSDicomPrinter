namespace TheSSS.DICOMViewer.Integration.Models
{
    /// <summary>
    /// Represents credentials for an external service.
    /// Can hold username/password, API key, token, etc.
    /// Used by ICredentialManager.
    /// </summary>
    public class ServiceCredentials
    {
        public string? Username { get; set; }
        public string? Password { get; set; } // Should be handled securely (e.g., SecureString in memory if possible)
        public string? ApiKey { get; set; }
        public string? Token { get; set; } // For Bearer tokens, OAuth tokens, etc.
        public string? Scheme { get; set; } // e.g. "Bearer" for Token

        // Constructor for simple username/password
        public ServiceCredentials(string username, string password)
        {
            Username = username;
            Password = password;
        }

        // Constructor for API Key
        public ServiceCredentials(string apiKey)
        {
            ApiKey = apiKey;
        }
        
        // Constructor for Token
        public ServiceCredentials(string token, string scheme)
        {
            Token = token;
            Scheme = scheme;
        }

        // Default constructor
        public ServiceCredentials() { }
    }
}