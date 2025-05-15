using System;

namespace TheSSS.DICOMViewer.Integration.Configuration;

/// <summary>
/// Configuration settings for the ICredentialManager.
/// Specifies secure store type, connection details, or rotation policy parameters.
/// </summary>
public class CredentialManagerSettings
{
    /// <summary>
    /// Gets or sets the type of secure store to use for credentials (e.g., "EnvironmentVariables", "DPAPI", "AzureKeyVault").
    /// This string identifier will determine the strategy used by CredentialManager.
    /// </summary>
    public string StoreType { get; set; } = "EnvironmentVariables"; // Default to environment variables for ease of use

    /// <summary>
    /// Gets or sets an optional path or connection string for the secure store, if applicable (e.g., path to a DPAPI-encrypted file, Key Vault URI).
    /// </summary>
    public string SecureStorePath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the CredentialManager should periodically check for rotated credentials.
    /// Actual rotation mechanism is outside the scope of simple retrieval but this flag can enable more proactive fetching.
    /// </summary>
    public bool EnableCredentialRotationChecks { get; set; } = false; // Default to false as rotation is complex

    /// <summary>
    /// Gets or sets the duration for which retrieved credentials should be cached in memory.
    /// This helps reduce calls to the underlying secure store.
    /// </summary>
    public TimeSpan CredentialCacheDuration { get; set; } = TimeSpan.FromMinutes(5);

    /// <summary>
    /// Gets or sets the interval at which to check for credential rotation if EnableCredentialRotationChecks is true.
    /// This property is only relevant if a background rotation check mechanism is implemented.
    /// </summary>
    public TimeSpan RotationCheckInterval { get; set; } = TimeSpan.FromHours(1);
}