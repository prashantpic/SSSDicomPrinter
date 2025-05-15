namespace TheSSS.DICOMViewer.Integration.Configuration;

/// <summary>
/// Configuration settings for DICOM network operations managed or coordinated by the gateway.
/// Details retry policies, timeouts, and concurrency settings.
/// </summary>
public class DicomGatewaySettings
{
    /// <summary>
    /// Gets or sets the key for retrieving the specific resilience policy for DICOM network operations from IResiliencePolicyProvider.
    /// This policy would cover aspects like retries and circuit breakers for DICOM calls.
    /// Corresponds to REQ-DNSPI-007.
    /// </summary>
    public string PolicyKey { get; set; } = "DicomNetworkPolicy";

    /// <summary>
    /// Gets or sets the resource key for rate limiting DICOM network operations, used by IRateLimiter.
    /// </summary>
    public string RateLimitResourceKey { get; set; } = "DicomNetwork";

    /// <summary>
    /// Gets or sets the maximum number of concurrent DICOM operations allowed from this client-side gateway.
    /// Corresponds to REQ-DNSPI-009.
    /// </summary>
    public int MaxConcurrentOperations { get; set; } = 5; // Default value
}