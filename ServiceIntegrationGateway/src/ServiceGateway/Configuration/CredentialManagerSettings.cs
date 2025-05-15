using System;

namespace TheSSS.DICOMViewer.Integration.Configuration
{
    /// <summary>
    /// Configuration settings for the ICredentialManager.
    /// </summary>
    public class CredentialManagerSettings
    {
        /// <summary>
        /// The type of secure store to use for retrieving credentials.
        /// Examples: "EnvironmentVariables", "AzureKeyVault", "HashiCorpVault", "DPAPI", "CustomSecureStore".
        /// The actual implementation will depend on an ISecureStore interface from REPO-CROSS-CUTTING.
        /// </summary>
        public string StoreType { get; set; } = "EnvironmentVariables";

        /// <summary>
        /// Path or connection string for the secure store, if applicable (e.g., KeyVault URI).
        /// For "EnvironmentVariables", this might be a prefix for variable names.
        /// </summary>
        public string SecureStorePathOrPrefix { get; set; } = string.Empty;

        /// <summary>
        /// Specifies if credential caching is enabled within the CredentialManager.
        /// </summary>
        public bool EnableCaching { get; set; } = true;

        /// <summary>
        /// Duration for which credentials should be cached if caching is enabled.
        /// Format: "00:05:00" for 5 minutes.
        /// </summary>
        public TimeSpan CacheDuration { get; set; } = TimeSpan.FromMinutes(5);

        /// <summary>
        /// If true, the CredentialManager might attempt to proactively refresh credentials
        /// before they expire, if the underlying store supports such metadata.
        /// </summary>
        public bool EnableProactiveRotation { get; set; } = false;

        /// <summary>
        /// Interval for checking for credential rotation if EnableProactiveRotation is true.
        /// Format: "01:00:00" for 1 hour.
        /// </summary>
        public TimeSpan RotationCheckInterval { get; set; } = TimeSpan.FromHours(1);
    }
}