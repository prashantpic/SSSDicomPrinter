using System;
using Polly; // For IAsyncPolicy

namespace TheSSS.DICOMViewer.Integration.Interfaces
{
    /// <summary>
    /// Interface for providing configured Polly resilience policies (e.g., Circuit Breaker, Retry, Timeout)
    /// identified by a policy key.
    /// </summary>
    public interface IResiliencePolicyProvider
    {
        /// <summary>
        /// Retrieves a pre-configured asynchronous Polly policy by its key.
        /// </summary>
        /// <param name="policyKey">The key identifying the desired policy (e.g., from PolicyRegistryKeys).</param>
        /// <returns>An IAsyncPolicy instance.</returns>
        /// <exception cref="PolicyNotFoundException">Thrown if a policy with the given key is not configured.</exception>
        IAsyncPolicy GetPolicyAsync(string policyKey);
    }

    /// <summary>
    /// Custom exception for when a requested policy is not found.
    /// </summary>
    [Serializable]
    public class PolicyNotFoundException : Exception
    {
        public string PolicyKey { get; }

        public PolicyNotFoundException() : base() { PolicyKey = string.Empty; }
        public PolicyNotFoundException(string policyKey) : base($"Resilience policy with key '{policyKey}' was not found.")
        {
            PolicyKey = policyKey;
        }
        public PolicyNotFoundException(string policyKey, string message) : base(message)
        {
            PolicyKey = policyKey;
        }
        public PolicyNotFoundException(string policyKey, string message, Exception inner) : base(message, inner)
        {
            PolicyKey = policyKey;
        }
        protected PolicyNotFoundException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context)
        {
            PolicyKey = info.GetString(nameof(PolicyKey)) ?? string.Empty;
        }

        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(PolicyKey), PolicyKey);
        }
    }
}