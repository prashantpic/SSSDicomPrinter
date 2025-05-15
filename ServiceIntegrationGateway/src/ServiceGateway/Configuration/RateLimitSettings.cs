using System;
using System.Collections.Generic;

namespace TheSSS.DICOMViewer.Integration.Configuration
{
    /// <summary>
    /// Configuration settings for API rate limiting policies.
    /// Allows defining different rate limits for different services or operations.
    /// </summary>
    public class RateLimitSettings
    {
        /// <summary>
        /// Specifies if rate limiting is enabled globally for the gateway.
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Dictionary of specific rate limit configurations.
        /// The key is a resource identifier (e.g., "OdooApi", "DicomCStorePerPeerX").
        /// </summary>
        public Dictionary<string, RateLimiterConfig> Limiters { get; set; } = new Dictionary<string, RateLimiterConfig>();
    }

    /// <summary>
    /// Configuration for a single rate limiter.
    /// </summary>
    public class RateLimiterConfig
    {
        /// <summary>
        /// The type of rate limiter to use.
        /// Supported types: "FixedWindow", "SlidingWindow", "TokenBucket", "Concurrency".
        /// </summary>
        public string Type { get; set; } = "TokenBucket";

        /// <summary>
        /// The maximum number of permits (requests) allowed in a given window or replenished.
        /// </summary>
        public int PermitLimit { get; set; } = 100;

        /// <summary>
        /// The time window for FixedWindow or SlidingWindow, or replenishment period for TokenBucket.
        /// Format: "00:01:00" for 1 minute.
        /// </summary>
        public TimeSpan Window { get; set; } = TimeSpan.FromMinutes(1);

        /// <summary>
        /// For TokenBucket: The number of tokens added per replenishment period (Window).
        /// For SlidingWindow: The number of segments in the window.
        /// </summary>
        public int? TokensPerPeriodOrSegments { get; set; }

        /// <summary>
        /// The maximum number of permits that can be queued if the limit is hit.
        /// 0 means no queueing (requests fail immediately or are rejected).
        /// </summary>
        public int QueueLimit { get; set; } = 0;

        /// <summary>
        /// How to process queued requests: "OldestFirst" or "NewestFirst".
        /// Applies if QueueLimit > 0.
        /// </summary>
        public string QueueProcessingOrder { get; set; } = "OldestFirst"; // OldestFirst, NewestFirst
    }
}