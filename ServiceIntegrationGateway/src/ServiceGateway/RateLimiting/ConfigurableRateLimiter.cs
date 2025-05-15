using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.RateLimiting;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using TheSSS.DICOMViewer.Integration.Configuration;
using TheSSS.DICOMViewer.Integration.Interfaces;
using TheSSS.DICOMViewer.CrossCutting.Logging; // Assuming ILoggerAdapter namespace

namespace TheSSS.DICOMViewer.Integration.RateLimiting
{
    public class ConfigurableRateLimiter : IRateLimiter, IAsyncDisposable
    {
        private readonly ILoggerAdapter<ConfigurableRateLimiter> _logger;
        private readonly ConcurrentDictionary<string, RateLimiter> _limiters = new();
        private readonly RateLimitSettings _globalRateLimitSettings;

        public ConfigurableRateLimiter(
            IOptions<RateLimitSettings> rateLimitSettings,
            ILoggerAdapter<ConfigurableRateLimiter> logger)
        {
            _logger = logger;
            _globalRateLimitSettings = rateLimitSettings.Value ?? new RateLimitSettings();

            InitializeLimiters();
        }

        private void InitializeLimiters()
        {
            _logger.LogInformation("Initializing rate limiters based on configuration.");
            if (_globalRateLimitSettings.Limits == null)
            {
                 _logger.LogWarning("No rate limits defined in configuration.");
                 return;
            }

            foreach (var kvp in _globalRateLimitSettings.Limits)
            {
                var resourceKey = kvp.Key;
                var limitSetting = kvp.Value;
                RateLimiter limiter = limitSetting.Type.ToLowerInvariant() switch
                {
                    "tokenbucket" => new TokenBucketRateLimiter(new TokenBucketRateLimiterOptions
                    {
                        TokenLimit = limitSetting.PermitLimit,
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = limitSetting.QueueLimit,
                        ReplenishmentPeriod = TimeSpan.FromMilliseconds(limitSetting.WindowMs),
                        TokensPerPeriod = limitSetting.TokensPerPeriod,
                        AutoReplenishment = true
                    }),
                    "fixedwindow" => new FixedWindowRateLimiter(new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = limitSetting.PermitLimit,
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = limitSetting.QueueLimit,
                        Window = TimeSpan.FromMilliseconds(limitSetting.WindowMs),
                        AutoReplenishment = true
                    }),
                    // Add other types like SlidingWindowRateLimiter, ConcurrencyLimiter if needed
                    _ => null
                };

                if (limiter != null)
                {
                    if (_limiters.TryAdd(resourceKey, limiter))
                    {
                        _logger.LogInformation("Initialized {Type} rate limiter for resource: {ResourceKey} (Limit: {PermitLimit}, Window: {WindowMs}ms, Queue: {QueueLimit})",
                            limitSetting.Type, resourceKey, limitSetting.PermitLimit, limitSetting.WindowMs, limitSetting.QueueLimit);
                    }
                    else
                    {
                        _logger.LogWarning("Could not add rate limiter for duplicate resource key: {ResourceKey}", resourceKey);
                        limiter.Dispose(); // Dispose if not added
                    }
                }
                else
                {
                    _logger.LogWarning("Unsupported rate limiter type '{Type}' for resource: {ResourceKey}", limitSetting.Type, resourceKey);
                }
            }
        }

        public async Task AcquirePermitAsync(string resourceKey, CancellationToken cancellationToken)
        {
            if (_limiters.TryGetValue(resourceKey, out var limiter))
            {
                RateLimitLease lease = await limiter.AcquireAsync(1, cancellationToken); // Acquire 1 permit
                if (!lease.IsAcquired)
                {
                    // This should not happen with AcquireAsync unless cancellation occurs before acquisition
                    // or if the limiter is disposed. Handle as an error or throw.
                    _logger.LogError("Failed to acquire rate limit permit for resource {ResourceKey}, lease was not acquired.", resourceKey);
                    throw new RateLimitLeaseNotAcquiredException($"Failed to acquire rate limit permit for {resourceKey}.");
                }
                // Lease will be disposed automatically if not used in a using statement.
                // For AcquireAsync, the permit is granted, or it throws/waits.
                // If you need to manually release: lease.Dispose(); but that's for try-acquire patterns.
            }
            else
            {
                // No specific limiter for this key, either allow or deny.
                // Default behavior: if not configured, allow. Or could use a global default limiter.
                _logger.LogDebug("No specific rate limiter configured for resource: {ResourceKey}. Call allowed without rate limiting.", resourceKey);
                // Optionally, apply a default global limiter if one is defined
                if (_globalRateLimitSettings.DefaultPermitLimit > 0 && _limiters.TryGetValue("DefaultGlobalRateLimit", out var defaultLimiter))
                {
                     RateLimitLease lease = await defaultLimiter.AcquireAsync(1, cancellationToken);
                     if(!lease.IsAcquired)
                     {
                        _logger.LogError("Failed to acquire default global rate limit permit for resource {ResourceKey}.", resourceKey);
                        throw new RateLimitLeaseNotAcquiredException($"Failed to acquire default global rate limit permit for {resourceKey}.");
                     }
                }
            }
        }

        public async ValueTask DisposeAsync()
        {
            _logger.LogInformation("Disposing ConfigurableRateLimiter and its underlying limiters.");
            foreach (var limiter in _limiters.Values)
            {
                await limiter.DisposeAsync();
            }
            _limiters.Clear();
        }
    }

    public class RateLimitLeaseNotAcquiredException : Exception
    {
        public RateLimitLeaseNotAcquiredException(string message) : base(message) { }
    }
}