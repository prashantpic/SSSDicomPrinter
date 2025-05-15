namespace TheSSS.DICOMViewer.Integration.Policies;

/// <summary>
/// Provides a centralized, type-safe way to reference specific resilience policies 
/// (e.g., 'OdooApiRetryPolicy', 'DicomCircuitBreakerPolicy') throughout the gateway.
/// </summary>
public static class PolicyRegistryKeys
{
    /// <summary>
    /// Default retry policy for general transient errors.
    /// </summary>
    public const string DefaultRetryPolicy = "DefaultRetryPolicy";

    /// <summary>
    /// Default circuit breaker policy for general service unavailability.
    /// </summary>
    public const string DefaultCircuitBreakerPolicy = "DefaultCircuitBreakerPolicy";

    /// <summary>
    /// Default timeout policy.
    /// </summary>
    public const string DefaultTimeoutPolicy = "DefaultTimeoutPolicy";

    /// <summary>
    /// Resilience policy specifically for Odoo API calls.
    /// </summary>
    public const string OdooApiResiliencePolicy = "OdooApiResiliencePolicy";

    /// <summary>
    /// Resilience policy specifically for SMTP service calls.
    /// </summary>
    public const string SmtpServiceResiliencePolicy = "SmtpServiceResiliencePolicy";

    /// <summary>
    /// General resilience policy for DICOM network operations.
    /// </summary>
    public const string DicomNetworkResiliencePolicy = "DicomNetworkResiliencePolicy";
    
    /// <summary>
    /// Resilience policy specifically for DICOM C-STORE operations.
    /// </summary>
    public const string DicomCStoreResiliencePolicy = "DicomCStoreResiliencePolicy";

    /// <summary>
    /// Resilience policy specifically for DICOM C-ECHO operations.
    /// </summary>
    public const string DicomCEchoResiliencePolicy = "DicomCEchoResiliencePolicy";

    /// <summary>
    /// Resilience policy specifically for DICOM C-FIND operations.
    /// </summary>
    public const string DicomCFindResiliencePolicy = "DicomCFindResiliencePolicy";

    /// <summary>
    /// Resilience policy specifically for DICOM C-MOVE operations.
    /// </summary>
    public const string DicomCMoveResiliencePolicy = "DicomCMoveResiliencePolicy";
}