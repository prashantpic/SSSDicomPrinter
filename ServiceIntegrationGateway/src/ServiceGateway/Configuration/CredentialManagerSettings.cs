namespace TheSSS.DICOMViewer.Integration.Configuration
{
    /// <summary>
    /// Configuration settings for the ICredentialManager.
    /// Specifies secure store type, connection details, or rotation policy parameters.
    /// </summary>
    public class CredentialManagerSettings
    {
        /// <summary>
        /// Type of secure store being used (e.g., "EnvironmentVariables", "DpApiFile", "AzureKeyVault").
        /// The actual implementation of ICredentialManager will interpret this.
        /// </summary>
        public string SecureStoreType { get; set; } = "EnvironmentVariables";

        /// <summary>
        /// Path or connection string for the secure store, if applicable (e.g., path to a DPAPI-protected file).
        /// </summary>
        public string? StoreLocation { get; set; }

        /// <summary>
        /// Prefix for environment variables if SecureStoreType is "EnvironmentVariables".
        /// E.g., if Prefix is "SVC_GATEWAY_", then for service "OdooApi", username might be "SVC_GATEWAY_OdooApi_Username".
        /// </summary>
        public string? EnvironmentVariablePrefix { get; set; } = "DICOMVIEWER_INTEGRATION_";

        /// <summary>
        /// Duration in seconds to cache credentials in memory. Zero or negative means no caching.
        /// </summary>
        public int CacheDurationSeconds { get; set; } = 300; // 5 minutes
    }
}