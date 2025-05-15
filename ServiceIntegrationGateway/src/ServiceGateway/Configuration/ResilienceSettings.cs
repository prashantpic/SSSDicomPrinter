using System;
using System.Collections.Generic;

namespace TheSSS.DICOMViewer.Integration.Configuration
{
    /// <summary>
    /// Configuration settings for Polly resilience policies.
    /// This allows defining multiple named policies that can be applied to different services.
    /// </summary>
    public class ResilienceSettings
    {
        /// <summary>
        /// Dictionary of named resilience policy configurations.
        /// The key is the policy name (e.g., "OdooApiPolicy", "DefaultDicomPolicy").
        /// </summary>
        public Dictionary<string, PolicyConfig> Policies { get; set; } = new Dictionary<string, PolicyConfig>();
    }

    /// <summary>
    /// Configuration for a single named Polly policy, which can include Retry, CircuitBreaker, and Timeout settings.
    /// </summary>
    public class PolicyConfig
    {
        /// <summary>
        /// Retry policy configuration. If null, no retry policy is applied for this named policy.
        /// </summary>
        public RetryConfig? Retry { get; set; }

        /// <summary>
        /// Circuit Breaker policy configuration. If null, no circuit breaker is applied.
        /// </summary>
        public CircuitBreakerConfig? CircuitBreaker { get; set; }

        /// <summary>
        /// Timeout policy configuration. If null, no specific timeout policy is applied (rely on underlying client timeout).
        /// </summary>
        public TimeoutConfig? Timeout { get; set; }
    }

    /// <summary>
    /// Configuration for a Retry policy.
    /// </summary>
    public class RetryConfig
    {
        /// <summary>
        /// Number of retry attempts.
        /// </summary>
        public int Count { get; set; } = 3;

        /// <summary>
        /// Base delay for backoff. For exponential backoff, this is the initial delay.
        /// Format: "00:00:01" for 1 second.
        /// </summary>
        public TimeSpan BaseDelay { get; set; } = TimeSpan.FromSeconds(1);

        /// <summary>
        /// Backoff strategy type: "Constant", "Linear", "Exponential".
        /// </summary>
        public string BackoffType { get; set; } = "Exponential"; // Constant, Linear, Exponential
    }

    /// <summary>
    /// Configuration for a Circuit Breaker policy.
    /// </summary>
    public class CircuitBreakerConfig
    {
        /// <summary>
        /// Number of consecutive exceptions allowed before the circuit breaks.
        /// </summary>
        public int ExceptionsAllowedBeforeBreaking { get; set; } = 5;

        /// <summary>
        /// Duration the circuit stays open after breaking.
        /// Format: "00:00:30" for 30 seconds.
        /// </summary>
        public TimeSpan DurationOfBreak { get; set; } = TimeSpan.FromSeconds(30);
    }

    /// <summary>
    /// Configuration for a Timeout policy.
    /// </summary>
    public class TimeoutConfig
    {
        /// <summary>
        /// Overall timeout for an operation wrapped by this policy.
        /// Format: "00:01:00" for 1 minute.
        /// </summary>
        public TimeSpan TimeoutValue { get; set; } = TimeSpan.FromSeconds(30);
    }
}