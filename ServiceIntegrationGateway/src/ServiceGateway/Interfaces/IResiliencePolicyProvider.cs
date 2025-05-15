using Polly;

namespace TheSSS.DICOMViewer.Integration.Interfaces;

/// <summary>
/// Defines a contract for obtaining specific, pre-configured Polly IAsyncPolicy instances (Circuit Breaker, Retry, Timeout) 
/// based on a provided key, allowing tailored resilience for different services. This interface is for providing configured 
/// Polly resilience policies.
/// </summary>
public interface IResiliencePolicyProvider
{
    /// <summary>
    /// Gets an asynchronous resilience policy based on a key.
    /// </summary>
    /// <param name="policyKey">The key identifying the desired policy configuration.</param>
    /// <returns>An IAsyncPolicy configured for the given key.</returns>
    IAsyncPolicy GetPolicyAsync(string policyKey);
}