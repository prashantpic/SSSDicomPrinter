namespace TheSSS.DICOMViewer.Integration.Policies;

/// <summary>
/// Provides a centralized, type-safe way to reference specific resilience policies 
/// (e.g., 'OdooApiRetryPolicy', 'DicomCircuitBreakerPolicy') throughout the gateway.
/// This class defines static constant string keys used to identify and retrieve specific Polly policies 
/// from the IResiliencePolicyProvider or a PolicyRegistry.
/// </summary>
public static class PolicyRegistryKeys
{
    /// <summary>
    /// Policy key for Odoo API interactions.
    /// </summary>
    public const string OdooApiPolicy = "OdooApiPolicy";

    /// <summary>
    /// Policy key for SMTP service interactions.
    /// </summary>
    public const string SmtpPolicy = "SmtpPolicy";

    /// <summary>
    /// Policy key for Windows Print service interactions (if resilience is applied).
    /// </summary>
    public const string WindowsPrintPolicy = "WindowsPrintPolicy";

    /// <summary>
    /// Policy key for DICOM network operations.
    /// </summary>
    public const string DicomNetworkPolicy = "DicomNetworkPolicy";

    // Add other policy keys as needed for different services or operations
    // Example:
    // public const string DicomCircuitBreakerPolicy = "DicomCircuitBreakerPolicy";
}