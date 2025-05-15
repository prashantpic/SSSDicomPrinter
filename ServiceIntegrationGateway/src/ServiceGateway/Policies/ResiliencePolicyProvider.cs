using System;
using System.Collections.Concurrent;
using System.Linq;
using Microsoft.Extensions.Options;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Polly.Timeout;
using TheSSS.DICOMViewer.Integration.Configuration;
using TheSSS.DICOMViewer.Integration.Interfaces;
using TheSSS.DICOMViewer.Common.Interfaces; // Assuming ILoggerAdapter is here

namespace TheSSS.DICOMViewer.Integration.Policies;

public class ResiliencePolicyProvider : IResiliencePolicyProvider
{
    private readonly ResilienceSettings _settings;
    private readonly ILoggerAdapter _logger;
    private readonly ConcurrentDictionary<string, IAsyncPolicy> _policyCache = new ConcurrentDictionary<string, IAsyncPolicy>();

    public ResiliencePolicyProvider(IOptions<ResilienceSettings> settings, ILoggerAdapter logger)
    {
        _settings = settings.Value ?? throw new ArgumentNullException(nameof(settings));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        BuildAndCachePolicies();
    }

    private void BuildAndCachePolicies()
    {
        if (_settings?.Policies == null || !_settings.Policies.Any())
        {
            _logger.Warning("ResilienceSettings.Policies is null or empty. No custom policies will be configured. Defaulting to NoOp.");
            return;
        }

        foreach (var policyConfig in _settings.Policies)
        {
            if (string.IsNullOrWhiteSpace(policyConfig.Key))
            {
                _logger.Warning("Skipping policy configuration with empty key.");
                continue;
            }

            var policyBuilder = Policy.WrapAsync(); // Start with a NoOp policy to wrap others around

            // Order of wrapping matters: Timeout should be outermost, then Retry, then CircuitBreaker
            // Or, if using PolicyWrap: Timeout -> Retry -> CircuitBreaker (inner-most)
            // Policy.WrapAsync(outer, inner) means outer executes first (e.g. timeout).
            // Let's build from inner to outer for clarity.

            IAsyncPolicy? currentPolicy = null;

            // 1. Circuit Breaker (Innermost)
            if (policyConfig.CircuitBreaker.Enabled)
            {
                currentPolicy = Policy
                    .Handle<Exception>(e => !(e is OperationCanceledException)) // Don't break on cancellation
                    .CircuitBreakerAsync(
                        policyConfig.CircuitBreaker.ExceptionsAllowedBeforeBreaking,
                        policyConfig.CircuitBreaker.DurationOfBreak,
                        onBreak: (exception, breakDelay, context) =>
                        {
                            _logger.Error(exception, $"Circuit breaker for policy '{policyConfig.Key}' (context: {context.OperationKey}) is opening for {breakDelay.TotalSeconds}s.");
                        },
                        onReset: (context) =>
                        {
                            _logger.Information($"Circuit breaker for policy '{policyConfig.Key}' (context: {context.OperationKey}) is resetting.");
                        },
                        onHalfOpen: () =>
                        {
                            _logger.Information($"Circuit breaker for policy '{policyConfig.Key}' is transitioning to half-open.");
                        });
            }

            // 2. Retry (Wraps Circuit Breaker or is standalone)
            if (policyConfig.Retry.Enabled)
            {
                var retryPolicy = Policy
                     .Handle<Exception>(e => !(e is OperationCanceledException || e is BrokenCircuitException)) // Don't retry on cancellation or if circuit is broken
                     .WaitAndRetryAsync(
                         policyConfig.Retry.RetryCount,
                         retryAttempt => TimeSpan.FromSeconds(Math.Pow(policyConfig.Retry.SleepDurationFactor.TotalSeconds, retryAttempt))
                             + TimeSpan.FromMilliseconds(new Random().Next(0, policyConfig.Retry.MaxJitterMilliseconds)),
                         (exception, timeSpan, retryCount, context) =>
                         {
                             _logger.Warning(exception, $"Retry #{retryCount} for policy '{policyConfig.Key}' (context: {context.OperationKey}) due to: {exception.GetType().Name}. Waiting {timeSpan.TotalSeconds:N1}s.");
                         });
                
                currentPolicy = currentPolicy != null ? retryPolicy.WrapAsync(currentPolicy) : retryPolicy;
            }

            // 3. Timeout (Outermost)
            if (policyConfig.Timeout.Enabled)
            {
                 var timeoutPolicy = Policy.TimeoutAsync(
                     policyConfig.Timeout.Timeout,
                     TimeoutStrategy.Optimistic, // For genuinely async operations
                     onTimeoutAsync: (context, timespan, task, exception) =>
                     {
                         _logger.Warning(exception, $"Operation timed out after {timespan.TotalSeconds}s for policy '{policyConfig.Key}' (context: {context.OperationKey}). Task status: {task?.Status}");
                         // The task is already faulted with TimeoutRejectedException by Polly
                         return Task.CompletedTask;
                     });

                currentPolicy = currentPolicy != null ? timeoutPolicy.WrapAsync(currentPolicy) : timeoutPolicy;
            }

            if (currentPolicy == null)
            {
                 currentPolicy = Policy.NoOpAsync(); // Fallback if no policies enabled for the key
                 _logger.Information($"Policy '{policyConfig.Key}' has no specific resilience mechanisms enabled; using NoOp policy.");
            }
            
            // Add a Context with the policy key for logging within Polly's handlers
            // This context can be passed when calling policy.ExecuteAsync(ctx => ..., context)
            // However, for GetPolicyAsync, we return the policy, and the caller provides context.
            // We can't pre-bake context here easily unless it's always the same.
            // The context logging in OnBreak/OnRetry etc. will use the context provided at execution time.

            _policyCache.AddOrUpdate(policyConfig.Key, currentPolicy, (key, existingPolicy) => currentPolicy);
            _logger.Information($"Configured and cached resilience policy for key: '{policyConfig.Key}'.");
        }
    }

    public IAsyncPolicy GetPolicyAsync(string policyKey)
    {
        if (_policyCache.TryGetValue(policyKey, out var policy))
        {
            return policy;
        }

        // If a policy key is requested but not found, it's a configuration error.
        // It's safer to throw than to return a NoOp policy silently, as this might hide issues.
        _logger.Error($"Resilience policy with key '{policyKey}' not found in configuration or cache.");
        throw new PolicyNotFoundException($"Resilience policy with key '{policyKey}' not found. Ensure it is configured in ResilienceSettings.");
    }
}

// Custom exception for missing policy configuration
public class PolicyNotFoundException : KeyNotFoundException
{
    public PolicyNotFoundException(string message) : base(message) { }
}