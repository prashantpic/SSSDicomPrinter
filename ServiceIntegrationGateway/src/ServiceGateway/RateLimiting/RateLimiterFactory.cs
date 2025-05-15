using Microsoft.Extensions.Options;
using TheSSS.DICOMViewer.Common.Interfaces; // Assuming ILoggerAdapter is here
using TheSSS.DICOMViewer.Integration.Configuration;
using TheSSS.DICOMViewer.Integration.Interfaces;

namespace TheSSS.DICOMViewer.Integration.RateLimiting;

/// <summary>
/// Factory class responsible for creating IRateLimiter instances.
/// It centralizes the creation of the primary rate limiter component, which internally manages
/// different rate limiters for various resource keys based on RateLimitSettings.
/// </summary>
public class RateLimiterFactory
{
    private readonly IOptions<RateLimitSettings> _settings;
    private readonly ILoggerAdapter _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="RateLimiterFactory"/> class.
    /// </summary>
    /// <param name="settings">The rate limit settings.</param>
    /// <param name="logger">The logger adapter.</param>
    public RateLimiterFactory(IOptions<RateLimitSettings> settings, ILoggerAdapter logger)
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Creates and configures the primary IRateLimiter instance.
    /// </summary>
    /// <returns>A configured IRateLimiter instance (typically <see cref="ConfigurableRateLimiter"/>).</returns>
    public IRateLimiter CreateRateLimiter()
    {
        // The ConfigurableRateLimiter itself handles the logic of reading settings
        // and creating specific System.Threading.RateLimiting.RateLimiter instances
        // for each configured resource key. This factory's role here is primarily
        // to instantiate this central ConfigurableRateLimiter.
        return new ConfigurableRateLimiter(_settings, _logger);
    }
}