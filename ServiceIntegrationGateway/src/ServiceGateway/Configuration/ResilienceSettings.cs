using System;
using System.Collections.Generic;

namespace TheSSS.DICOMViewer.Integration.Configuration;

/// <summary>
/// Configuration settings for Polly resilience policies (Circuit Breaker, Retry, Timeout)
/// applicable to various services, keyed by service type or specific policy identifiers.
/// </summary>
public class ResilienceSettings
{
    /// <summary>
    /// Gets or sets a list of named policy configurations.
    /// </summary>
    public List<PolicyConfiguration> Policies { get; set; } = new List<PolicyConfiguration>();
}

/// <summary>
/// Defines settings for a specific named resilience policy.
/// </summary>
public class PolicyConfiguration
{
    /// <summary>
    /// Gets or sets the unique key for this policy configuration (e.g., "OdooApiPolicy", "DicomNetworkPolicy").
    /// This key is used to retrieve the policy from the IResiliencePolicyProvider.
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the settings for the Retry policy component.
    /// </summary>
    public RetryPolicySettings Retry { get; set; } = new RetryPolicySettings();

    /// <summary>
    /// Gets or sets the settings for the Circuit Breaker policy component.
    /// </summary>
    public CircuitBreakerPolicySettings CircuitBreaker { get; set; } = new CircuitBreakerPolicySettings();

    /// <summary>
    /// Gets or sets the settings for the Timeout policy component.
    /// </summary>
    public TimeoutPolicySettings Timeout { get; set; } = new TimeoutPolicySettings();
}

/// <summary>
/// Settings for a Retry policy.
/// </summary>
public class RetryPolicySettings
{
    /// <summary>
    /// Gets or sets a value indicating whether the retry policy is enabled.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Gets or sets the number of retry attempts.
    /// </summary>
    public int RetryCount { get; set; } = 3;

    /// <summary>
    /// Gets or sets the base duration for exponential backoff (e.g., 1 second).
    /// The actual sleep duration will be SleepDurationFactor ^ retryAttempt.
    /// </summary>
    public TimeSpan SleepDurationFactor { get; set; } = TimeSpan.FromSeconds(2);

    /// <summary>
    /// Gets or sets the maximum jitter in milliseconds to add to the sleep duration,
    /// helping to prevent thundering herd scenarios.
    /// </summary>
    public int MaxJitterMilliseconds { get; set; } = 100;
}

/// <summary>
/// Settings for a Circuit Breaker policy.
/// </summary>
public class CircuitBreakerPolicySettings
{
    /// <summary>
    /// Gets or sets a value indicating whether the circuit breaker policy is enabled.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Gets or sets the number of exceptions allowed before breaking the circuit.
    /// </summary>
    public int ExceptionsAllowedBeforeBreaking { get; set; } = 5;

    /// <summary>
    /// Gets or sets the duration for which the circuit will remain broken.
    /// </summary>
    public TimeSpan DurationOfBreak { get; set; } = TimeSpan.FromSeconds(30);
}

/// <summary>
/// Settings for a Timeout policy.
/// </summary>
public class TimeoutPolicySettings
{
    /// <summary>
    /// Gets or sets a value indicating whether the timeout policy is enabled.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Gets or sets the timeout duration for an operation.
    /// </summary>
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
}