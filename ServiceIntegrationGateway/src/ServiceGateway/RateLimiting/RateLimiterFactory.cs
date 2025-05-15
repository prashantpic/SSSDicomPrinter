using Microsoft.Extensions.Options;
using TheSSS.DICOMViewer.Integration.Configuration; // Assuming RateLimitSettings is here
using TheSSS.DICOMViewer.Integration.Interfaces; // For ILoggerAdapter, IRateLimiter
// Placeholder for ILoggerAdapter if it's from REPO-CROSS-CUTTING, e.g.
// using TheSSS.DICOMViewer.CrossCutting.Logging; 

namespace TheSSS.DICOMViewer.Integration.RateLimiting;

// Forward declaration for ILoggerAdapter if not available in this context.
// This would typically come from a shared cross-cutting concerns library.
// namespace TheSSS.DICOMViewer.CrossCutting.Logging
// {
//    public interface ILoggerAdapter<T> { void LogError(string message); /* more methods */ }
// }

// Forward declaration for RateLimitSettings and its inner classes.
// These would typically be defined in TheSSS.DICOMViewer.Integration.Configuration.
// namespace TheSSS.DICOMViewer.Integration.Configuration
// {
//     public class RateLimitSettings 
//     {
//         public Dictionary<string, RateLimitRuleSetting> Rules { get; set; } = new();
//     }
//     public class RateLimitRuleSetting 
//     {
//         public string LimiterType { get; set; } = "FixedWindow"; // e.g. TokenBucket, FixedWindow, SlidingWindow, Concurrency
//         public int PermitLimit { get; set; } = 100;
//         public int WindowSeconds { get; set; } = 60; // For windowed limiters
//         public int QueueLimit { get; set; } = 0;
//         public int TokensPerPeriod { get; set; } = 10; // For TokenBucket
//         public int ReplenishmentPeriodSeconds { get; set; } = 1; // For TokenBucket
//     }
// }


/// <summary>
/// Factory class responsible for creating and configuring IRateLimiter instances.
/// It reads RateLimitSettings to configure different rate limiters for various 
/// resource keys (services/operations).
/// This factory will construct the primary IRateLimiter implementation (ConfigurableRateLimiter).
/// </summary>
public class RateLimiterFactory
{
    private readonly IOptions<RateLimitSettings> _rateLimitSettings;
    private readonly ILoggerAdapter<ConfigurableRateLimiter> _logger; // Logger for the ConfigurableRateLimiter

    /// <summary>
    /// Initializes a new instance of the <see cref="RateLimiterFactory"/> class.
    /// </summary>
    /// <param name="rateLimitSettings">The rate limit configuration settings.</param>
    /// <param name="logger">The logger to be used by the created rate limiter.</param>
    public RateLimiterFactory(
        IOptions<RateLimitSettings> rateLimitSettings,
        ILoggerAdapter<ConfigurableRateLimiter> logger) // ConfigurableRateLimiter will be the one logging
    {
        _rateLimitSettings = rateLimitSettings ?? throw new ArgumentNullException(nameof(rateLimitSettings));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Creates and configures an instance of <see cref="IRateLimiter"/>.
    /// The returned <see cref="IRateLimiter"/> (typically <see cref="ConfigurableRateLimiter"/>)
    /// will internally manage specific rate limiters for different resource keys based on configuration.
    /// </summary>
    /// <returns>A configured instance of <see cref="IRateLimiter"/>.</returns>
    public IRateLimiter CreateRateLimiter()
    {
        // ConfigurableRateLimiter will take IOptions<RateLimitSettings> and ILoggerAdapter
        // and initialize its internal limiters based on the settings.
        return new ConfigurableRateLimiter(_rateLimitSettings, _logger);
    }
}