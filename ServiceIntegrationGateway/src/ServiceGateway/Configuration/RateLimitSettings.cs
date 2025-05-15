using System;
using System.Collections.Generic;
using System.Threading.RateLimiting;

namespace TheSSS.DICOMViewer.Integration.Configuration;

/// <summary>
/// Configuration settings for API rate limiting policies.
/// Allows different limits for different services or operations, defined by requests per time window.
/// </summary>
public class RateLimitSettings
{
    /// <summary>
    /// Gets or sets a value indicating whether rate limiting is globally enabled for services managed by this gateway.
    /// If false, no rate limiting will be applied even if specific limiters are configured.
    /// </summary>
    public bool EnableRateLimitingPerService { get; set; } = true;

    /// <summary>
    /// Gets or sets a list of configurations for individual rate limiters.
    /// Each configuration defines limits for a specific resource key (e.g., "OdooApi", "DicomCStore").
    /// </summary>
    public List<RateLimiterConfig> Limiters { get; set; } = new List<RateLimiterConfig>();
}

/// <summary>
/// Defines the configuration for a specific rate limiter.
/// </summary>
public class RateLimiterConfig
{
    /// <summary>
    /// Gets or sets the unique key identifying the resource this rate limiter applies to (e.g., "OdooApi", "DicomNetwork").
    /// </summary>
    public string ResourceKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the mode of the rate limiter (e.g., FixedWindow, TokenBucket, Concurrency).
    /// </summary>
    public RateLimiterMode Mode { get; set; } = RateLimiterMode.FixedWindow;

    /// <summary>
    /// Gets or sets the permit limit.
    /// For FixedWindow/SlidingWindow: Max requests per window.
    /// For TokenBucket: Maximum number of tokens that can be stored.
    /// For Concurrency: Maximum concurrent requests.
    /// </summary>
    public int PermitLimit { get; set; } = 100;

    /// <summary>
    /// Gets or sets the time window for FixedWindow or SlidingWindow modes.
    /// Not used for TokenBucket (see ReplenishmentPeriod) or Concurrency modes.
    /// </summary>
    public TimeSpan Window { get; set; } = TimeSpan.FromMinutes(1);

    /// <summary>
    /// Gets or sets the maximum number of permits that can be queued when the limit is reached.
    /// Default is 0, meaning requests fail immediately if the limit is hit and queue is full or not configured.
    /// </summary>
    public int QueueLimit { get; set; } = 0;

    /// <summary>
    /// Gets or sets the processing order for queued requests (OldestFirst or NewestFirst).
    /// </summary>
    public QueueProcessingOrder QueueProcessingOrder { get; set; } = QueueProcessingOrder.OldestFirst;

    // TokenBucket specific settings
    /// <summary>
    /// Gets or sets the time interval at which tokens are replenished for TokenBucket mode.
    /// </summary>
    public TimeSpan ReplenishmentPeriod { get; set; } = TimeSpan.FromSeconds(10);

    /// <summary>
    /// Gets or sets the number of tokens added to the bucket during each replenishment period for TokenBucket mode.
    /// </summary>
    public int TokensPerPeriod { get; set; } = 10;

    /// <summary>
    /// Gets or sets a value indicating whether the token bucket should be automatically replenished.
    /// </summary>
    public bool AutoReplenishment { get; set; } = true;
}

/// <summary>
/// Specifies the type of rate limiter algorithm to use.
/// </summary>
public enum RateLimiterMode
{
    /// <summary>
    /// Limits requests within discrete, non-overlapping time windows.
    /// </summary>
    FixedWindow,
    /// <summary>
    /// Uses a bucket of tokens that refills over time, allowing bursts.
    /// </summary>
    TokenBucket,
    /// <summary>
    /// Limits the number of concurrent operations.
    /// </summary>
    Concurrency
    // SlidingWindow could be added if System.Threading.RateLimiting supports it or a custom one is built.
}