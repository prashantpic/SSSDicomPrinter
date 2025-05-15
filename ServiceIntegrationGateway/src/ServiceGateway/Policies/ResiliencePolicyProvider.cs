using System;
using System.Net.Http;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Contrib.WaitAndRetry; // For Backoff.DecorrelatedJitterBackoffV2
using Polly.Registry;
using Polly.Timeout;
using TheSSS.DICOMViewer.Integration.Configuration;
using TheSSS.DICOMViewer.Integration.Interfaces;
using TheSSS.DICOMViewer.CrossCutting.Logging; // Assuming ILoggerAdapter namespace
using System.Collections.Generic; // For IEnumerable
using System.Threading.Tasks; // For Task

namespace TheSSS.DICOMViewer.Integration.Policies
{
    public class ResiliencePolicyProvider : IResiliencePolicyProvider
    {
        private readonly IPolicyRegistry<string> _policyRegistry;
        private readonly ILoggerAdapter<ResiliencePolicyProvider> _logger;
        private readonly ResilienceSettings _resilienceSettings;

        public ResiliencePolicyProvider(
            IOptions<ResilienceSettings> resilienceSettings,
            ILoggerAdapter<ResiliencePolicyProvider> logger)
        {
            _logger = logger;
            _resilienceSettings = resilienceSettings.Value ?? new ResilienceSettings(); // Ensure settings are not null
            _policyRegistry = new PolicyRegistry();

            InitializePolicies();
        }

        private void InitializePolicies()
        {
            _logger.LogInformation("Initializing resilience policies.");

            // Default API Resilience Policy
            var defaultApiPolicySettings = _resilienceSettings.Policies.TryGetValue(PolicyRegistryKeys.DefaultApiResiliencePolicy, out var defaultSettings)
                ? defaultSettings : new ResiliencePolicySetting(); // Fallback to default settings

            var defaultApiPolicy = CreatePolicy(defaultApiPolicySettings, PolicyRegistryKeys.DefaultApiResiliencePolicy);
            _policyRegistry.Add(PolicyRegistryKeys.DefaultApiResiliencePolicy, defaultApiPolicy);

            // DICOM Network Resilience Policy
            var dicomNetworkPolicySettings = _resilienceSettings.Policies.TryGetValue(PolicyRegistryKeys.DicomNetworkResiliencePolicy, out var dicomSettings)
                ? dicomSettings : new ResiliencePolicySetting { RetryCount = 5 }; // Example specific default

            var dicomNetworkPolicy = CreatePolicy(dicomNetworkPolicySettings, PolicyRegistryKeys.DicomNetworkResiliencePolicy);
            _policyRegistry.Add(PolicyRegistryKeys.DicomNetworkResiliencePolicy, dicomNetworkPolicy);

            // Add other policies based on _resilienceSettings.Policies
            foreach (var policyConfigEntry in _resilienceSettings.Policies)
            {
                if (!_policyRegistry.ContainsKey(policyConfigEntry.Key)) // Avoid re-adding if already handled above
                {
                    var policy = CreatePolicy(policyConfigEntry.Value, policyConfigEntry.Key);
                    _policyRegistry.Add(policyConfigEntry.Key, policy);
                    _logger.LogInformation("Created and registered resilience policy: {PolicyKey}", policyConfigEntry.Key);
                }
            }
        }

        private IAsyncPolicy CreatePolicy(ResiliencePolicySetting settings, string policyKey)
        {
            // Retry Policy
            var retryPolicy = Policy
                .Handle<HttpRequestException>()
                .Or<TimeoutRejectedException>() // Thrown by Polly's TimeoutPolicy
                .Or<TaskCanceledException>(ex => ex.CancellationToken == default) // Handle non-cancellation token related task cancellations as transient
                // Add other transient exceptions specific to services, e.g., SmtpException, Dicom specific transient errors
                .WaitAndRetryAsync(
                    settings.RetryCount,
                    retryAttempt => Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromMilliseconds(settings.RetryDelayBaseMs), retryAttempt),
                    (exception, timespan, retryCount, context) =>
                    {
                        _logger.LogWarning(exception, "Policy {PolicyKey}: Retry {RetryCount} after {Timespan} due to {ExceptionType}.",
                                           policyKey, retryCount, timespan, exception.GetType().Name);
                    });

            // Circuit Breaker Policy
            var circuitBreakerPolicy = Policy
                .Handle<HttpRequestException>() // Define what exceptions trip the circuit
                .Or<TimeoutRejectedException>()
                .CircuitBreakerAsync(
                    settings.CircuitBreakerFailureThreshold,
                    TimeSpan.FromMilliseconds(settings.CircuitBreakerDurationOfBreakMs),
                    onBreak: (exception, duration, context) =>
                    {
                        _logger.LogWarning(exception, "Policy {PolicyKey}: Circuit breaker opened for {Duration} due to {ExceptionType}.",
                                           policyKey, duration, exception.GetType().Name);
                    },
                    onReset: (context) =>
                    {
                        _logger.LogInformation("Policy {PolicyKey}: Circuit breaker reset.", policyKey);
                    },
                    onHalfOpen: () =>
                    {
                        _logger.LogInformation("Policy {PolicyKey}: Circuit breaker half-open. Next call is a test.", policyKey);
                    });

            // Timeout Policy
            var timeoutPolicy = Policy.TimeoutAsync(TimeSpan.FromMilliseconds(settings.TimeoutMs));

            // Combine policies: Timeout wraps CircuitBreaker, which wraps Retry.
            // Order is important: outer policies execute first.
            // Timeout -> Retry -> CircuitBreaker (Retry should be inside CB so CB counts retried failures)
            // Or Timeout -> CircuitBreaker -> Retry (CB breaks, then retries are attempted after CB reset. This is usually preferred)
            
            // Let's use: Timeout -> CircuitBreaker -> Retry
            // The request flows inward:
            // 1. Timeout policy starts a timer.
            // 2. CircuitBreaker checks its state. If open, throws BrokenCircuitException. If closed/half-open, passes call.
            // 3. Retry policy executes the action. If it fails with a handled transient error, it retries.
            //    If retries are exhausted, the exception propagates to CircuitBreaker.
            // 4. CircuitBreaker notes the failure. If threshold met, it breaks.
            // 5. If timeout occurs at any point, TimeoutRejectedException is thrown.

            // Standard Polly wrapping order:
            // Timeout (outermost) -> Retry -> CircuitBreaker (innermost for action) - this is often debated.
            // Let's consider Polly docs recommendation: Retry -> CB -> Action
            // And Timeout can be outermost or innermost. Outermost timeout controls the whole operation including retries.
            // Example: Timeout > Retry > Circuit Breaker > Action (action is the actual HTTP call)

            return timeoutPolicy.WrapAsync(circuitBreakerPolicy.WrapAsync(retryPolicy));
        }

        public Task<IAsyncPolicy> GetPolicyAsync(string policyKey) // Task to align with potential async init in future
        {
            if (_policyRegistry.TryGet(policyKey, out IAsyncPolicy policy))
            {
                return Task.FromResult(policy);
            }

            _logger.LogWarning("Resilience policy with key '{PolicyKey}' not found. Falling back to default API policy.", policyKey);
            if (_policyRegistry.TryGet(PolicyRegistryKeys.DefaultApiResiliencePolicy, out IAsyncPolicy defaultPolicy))
            {
                 return Task.FromResult(defaultPolicy);
            }
            
            _logger.LogError("Default resilience policy not found either. Returning NoOp policy.");
            return Task.FromResult(Policy.NoOpAsync()); // Fallback to NoOp if even default is missing
        }
    }
}