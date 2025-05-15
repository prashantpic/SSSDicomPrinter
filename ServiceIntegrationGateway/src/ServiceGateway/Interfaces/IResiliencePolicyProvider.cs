using Polly;

namespace TheSSS.DICOMViewer.Integration.Interfaces;

/// <summary>
/// Defines a contract for obtaining specific, pre-configured Polly IAsyncPolicy instances 
/// (Circuit Breaker, Retry, Timeout) based on a provided key, 
/// allowing tailored resilience for different services.
/// </summary>
public interface IResiliencePolicyProvider
{
    /// <summary>
    /// Gets an asynchronous resilience policy by its key.
    /// </summary>
    /// <param name="policyKey">The key identifying the policy.</param>
    /// <returns>The configured Polly IAsyncPolicy.</returns>
    IAsyncPolicy GetPolicyAsync(string policyKey);
}