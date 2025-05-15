using System.Collections.Generic;

namespace TheSSS.DICOMViewer.Integration.Configuration
{
    /// <summary>
    /// Configuration settings for Polly resilience policies (Circuit Breaker, Retry, Timeout)
    /// applicable to various services, keyed by service type or specific policy identifiers.
    /// </summary>
    public class ResilienceSettings
    {
        public Dictionary<string, PolicySetting> Policies { get; set; } = new();
    }

    public class PolicySetting
    {
        public RetryPolicySetting? Retry { get; set; }
        public CircuitBreakerPolicySetting? CircuitBreaker { get; set; }
        public TimeoutPolicySetting? Timeout { get; set; } // Overall timeout per attempt
        public TimeoutPolicySetting? ExecutionTimeout { get; set; } // Timeout per single execution within retry
    }

    public class RetryPolicySetting
    {
        public int MaxRetries { get; set; } = 3;
        public List<int> RetryDelaysSeconds { get; set; } = [1, 3, 5]; // Example: exponential backoff
        public bool Jitter { get; set; } = true; // Whether to apply jitter to delays
    }

    public class CircuitBreakerPolicySetting
    {
        public double FailureThreshold { get; set; } = 0.5; // Proportion of failures before breaking
        public int MinimumThroughput { get; set; } = 5; // Min requests in sampling duration to evaluate
        public int SamplingDurationSeconds { get; set; } = 30; // Duration over which failures are counted
        public int DurationOfBreakSeconds { get; set; } = 60; // How long the circuit stays open
    }

    public class TimeoutPolicySetting
    {
        public int TimeoutSeconds { get; set; } = 30;
    }
}