using System.Collections.Generic;
using System.Threading.RateLimiting;

namespace TheSSS.DICOMViewer.Integration.Configuration
{
    /// <summary>
    /// Configuration settings for API rate limiting policies.
    /// Allows different limits for different services or operations, defined by requests per time window.
    /// </summary>
    public class RateLimitSettings
    {
        public Dictionary<string, ResourceRateLimitSetting> Resources { get; set; } = new();
    }

    public class ResourceRateLimitSetting
    {
        public RateLimiterType LimiterType { get; set; } = RateLimiterType.FixedWindow;
        public int PermitLimit { get; set; } = 100; // e.g., Tokens for TokenBucket, Permits for FixedWindow
        public int WindowSeconds { get; set; } = 60; // Replenishment period for TokenBucket, Window for FixedWindow
        
        // For TokenBucket
        public int TokensPerPeriod { get; set; } = 100;
        public int QueueLimit { get; set; } = 0; // Max number of permits to queue
        public QueueProcessingOrder QueueProcessingOrder { get; set; } = QueueProcessingOrder.OldestFirst;
        
        // For FixedWindow / SlidingWindow
        public int SegmentsPerWindow { get; set; } = 1; // For SlidingWindow
        
        // For ConcurrencyLimiter
        // PermitLimit can be used as MaxConcurrentRequests

        public bool AutoReplenishment { get; set; } = true; // For TokenBucket
    }

    public enum RateLimiterType
    {
        FixedWindow,
        SlidingWindow,
        TokenBucket,
        Concurrency
    }
}