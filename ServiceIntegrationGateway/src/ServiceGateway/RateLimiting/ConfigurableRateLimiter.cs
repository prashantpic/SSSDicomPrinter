using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.RateLimiting;
using Microsoft.Extensions.Options;
using TheSSS.DICOMViewer.Integration.Configuration;
using TheSSS.DICOMViewer.Integration.Interfaces;
using TheSSS.DICOMViewer.Common.Interfaces; // Assuming ILoggerAdapter is here

namespace TheSSS.DICOMViewer.Integration.RateLimiting;

public class ConfigurableRateLimiter : IRateLimiter, IAsyncDisposable
{
    private readonly RateLimitSettings _settings;
    private readonly ILoggerAdapter _logger;
    private readonly ConcurrentDictionary<string, System.Threading.RateLimiting.RateLimiter> _limiters = 
        new ConcurrentDictionary<string, System.Threading.RateLimiting.RateLimiter>();
    private bool _disposed = false;

    public ConfigurableRateLimiter(IOptions<RateLimitSettings> settings, ILoggerAdapter logger)
    {
        _settings = settings.Value ?? throw new ArgumentNullException(nameof(settings));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        InitializeLimiters();
    }

    private void InitializeLimiters()
    {
        if (!_settings.EnableRateLimitingPerService || _settings.Limiters == null)
        {
            _logger.Information("Rate limiting is globally disabled or no limiters are configured.");
            return;
        }

        foreach (var limiterConfig in _settings.Limiters)
        {
            if (string.IsNullOrWhiteSpace(limiterConfig.ResourceKey))
            {
                _logger.Warning("Skipping rate limiter configuration with empty resource key.");
                continue;
            }

            System.Threading.RateLimiting.RateLimiter? limiter = null;
            var options = new RateLimiterOptions
            {
                PermitLimit = limiterConfig.PermitLimit,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst, // Default, can be configured if needed
                QueueLimit = limiterConfig.QueueLimit
            };

            switch (limiterConfig.Mode)
            {
                case RateLimiterMode.FixedWindow:
                    limiter = new FixedWindowRateLimiter(new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = options.PermitLimit,
                        Window = limiterConfig.Window, // Specific to FixedWindow
                        QueueProcessingOrder = options.QueueProcessingOrder,
                        QueueLimit = options.QueueLimit,
                        AutoReplenishment = true // Default for FixedWindow
                    });
                    _logger.Information($"Configured FixedWindowRateLimiter for resource '{limiterConfig.ResourceKey}' with limit {options.PermitLimit}/{limiterConfig.Window.TotalSeconds}s, Queue: {options.QueueLimit}.");
                    break;
                case RateLimiterMode.TokenBucket:
                    limiter = new TokenBucketRateLimiter(new TokenBucketRateLimiterOptions
                    {
                        TokenLimit = options.PermitLimit, // Max tokens in bucket
                        QueueProcessingOrder = options.QueueProcessingOrder,
                        QueueLimit = options.QueueLimit,
                        ReplenishmentPeriod = limiterConfig.ReplenishmentPeriod, // Specific to TokenBucket
                        TokensPerPeriod = limiterConfig.TokensPerPeriod,       // Specific to TokenBucket
                        AutoReplenishment = limiterConfig.AutoReplenishment    // Specific to TokenBucket
                    });
                    _logger.Information($"Configured TokenBucketRateLimiter for resource '{limiterConfig.ResourceKey}' with limit {options.PermitLimit} tokens, replenishing {limiterConfig.TokensPerPeriod}/{limiterConfig.ReplenishmentPeriod.TotalSeconds}s, Queue: {options.QueueLimit}.");
                    break;
                case RateLimiterMode.Concurrency:
                    limiter = new ConcurrencyLimiter(new ConcurrencyLimiterOptions
                    {
                        PermitLimit = options.PermitLimit, // Max concurrent requests
                        QueueProcessingOrder = options.QueueProcessingOrder,
                        QueueLimit = options.QueueLimit
                    });
                    _logger.Information($"Configured ConcurrencyLimiter for resource '{limiterConfig.ResourceKey}' with limit {options.PermitLimit} concurrent operations, Queue: {options.QueueLimit}.");
                    break;
                default:
                    _logger.Warning($"Unknown RateLimiterMode '{limiterConfig.Mode}' for resource '{limiterConfig.ResourceKey}'. Skipping configuration.");
                    continue;
            }

            if (limiter != null)
            {
                var oldLimiter = _limiters.AddOrUpdate(limiterConfig.ResourceKey, limiter, (key, existingLimiter) =>
                {
                    // Asynchronously dispose the old limiter if it's being replaced
                    // This is complex with AddOrUpdate; easier to handle outside or assume AddOrUpdate is on init.
                    // For simplicity, direct replacement assuming initialization or infrequent updates.
                    // If dynamic reconfig is needed, careful disposal of old limiters is required.
                    _ = existingLimiter.DisposeAsync(); 
                    return limiter;
                });
                // If 'oldLimiter' was the same instance as 'limiter', it means it was an add, not an update.
                // If it was different, it was an update, and the old one is now being disposed.
            }
        }
    }

    public async Task AcquirePermitAsync(string resourceKey, CancellationToken cancellationToken = default)
    {
        if (!_settings.EnableRateLimitingPerService)
        {
            return; // Rate limiting is globally disabled.
        }

        if (_limiters.TryGetValue(resourceKey, out var limiter))
        {
            _logger.Debug($"Attempting to acquire rate limit permit for resource '{resourceKey}'. Stats: Available={limiter.GetStatistics()?.CurrentAvailablePermits}, Queued={limiter.GetStatistics()?.CurrentQueuedCount}.");
            
            RateLimitLease lease = default;
            try
            {
                lease = await limiter.AcquireAsync(1, cancellationToken).ConfigureAwait(false);

                if (lease.IsAcquired)
                {
                    _logger.Debug($"Successfully acquired rate limit permit for resource '{resourceKey}'.");
                }
                else
                {
                    _logger.Warning($"Failed to acquire rate limit permit for resource '{resourceKey}'. Lease not acquired (e.g., queue limit hit and timed out).");
                    // This situation (lease not acquired) implies request should not proceed.
                    // Throwing here helps propagate the failure clearly.
                    throw new RateLimitExceededException($"Failed to acquire rate limit permit for resource '{resourceKey}'. The request cannot be processed at this time.");
                }
            }
            catch (ArgumentOutOfRangeException aoore) when (aoore.ParamName == "permitCount") // Should not happen with permitCount = 1
            {
                _logger.Error(aoore, $"Internal error in rate limiter for resource '{resourceKey}': permitCount was invalid.");
                throw; // Re-throw as this is an unexpected internal issue.
            }
            catch (RateLimiterRejectedException rejectedEx)
            {
                 // This exception is thrown by some limiters (e.g., FixedWindow) if a permit cannot be leased
                 // (e.g., queue is full, or request cannot be satisfied within the limiter's constraints).
                 _logger.Warning(rejectedEx, $"Rate limit permit acquisition rejected for resource '{resourceKey}'.");
                 throw new RateLimitExceededException($"Rate limit rejected for resource '{resourceKey}'.", rejectedEx);
            }
            catch (OperationCanceledException)
            {
                _logger.Information($"Rate limit permit acquisition cancelled for resource '{resourceKey}'.");
                lease?.Dispose(); // Ensure lease is disposed if acquired partially or cancellation occurs during wait.
                throw;
            }
            catch (Exception ex) // Catch-all for unexpected errors during acquisition
            {
                _logger.Error(ex, $"An unexpected error occurred during rate limit permit acquisition for resource '{resourceKey}'.");
                lease?.Dispose();
                throw; // Re-throw to indicate a problem.
            }
            // Lease should be disposed by the caller if they need to hold it.
            // However, for simple acquire-and-proceed, the lease is typically short-lived (within this method's scope).
            // If AcquirePermitAsync's contract is just to "wait until permitted", the lease can be disposed here.
            // If the lease needs to be held by the caller (e.g., for ConcurrencyLimiter where lease.Dispose releases permit),
            // then IRateLimiter should return RateLimitLease.
            // Given the current IRateLimiter interface (Task), we assume the permit is acquired and immediately "used".
            // So, dispose the lease here.
            lease?.Dispose();
        }
        else
        {
            _logger.Debug($"No rate limiter configured for resource '{resourceKey}'. Proceeding without limiting.");
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (!_disposed)
        {
            foreach (var limiter in _limiters.Values)
            {
                await limiter.DisposeAsync().ConfigureAwait(false);
            }
            _limiters.Clear();
            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }
}

// Custom exception for rate limit exceeding
public class RateLimitExceededException : Exception
{
    public RateLimitExceededException(string message) : base(message) { }
    public RateLimitExceededException(string message, Exception innerException) : base(message, innerException) { }
}