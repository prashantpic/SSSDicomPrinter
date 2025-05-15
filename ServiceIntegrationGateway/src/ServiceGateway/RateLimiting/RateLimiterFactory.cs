using System;
using Microsoft.Extensions.Options;
using TheSSS.DICOMViewer.Common.Logging; // Assuming ILoggerAdapter is in this namespace
using TheSSS.DICOMViewer.Integration.Configuration;
using TheSSS.DICOMViewer.Integration.Interfaces;

namespace TheSSS.DICOMViewer.Integration.RateLimiting
{
    /// <summary>
    /// Factory class responsible for creating and configuring IRateLimiter instances.
    /// This factory centralizes the creation of the ConfigurableRateLimiter.
    /// </summary>
    public class RateLimiterFactory
    {
        private readonly IOptions<RateLimitSettings> _settings;
        private readonly ILoggerAdapter<ConfigurableRateLimiter> _configurableRateLimiterLogger; // Logger for the ConfigurableRateLimiter

        /// <summary>
        /// Initializes a new instance of the <see cref="RateLimiterFactory"/> class.
        /// </summary>
        /// <param name="settings">The rate limit settings.</param>
        /// <param name="configurableRateLimiterLogger">The logger for the ConfigurableRateLimiter that will be created.</param>
        public RateLimiterFactory(
            IOptions<RateLimitSettings> settings,
            ILoggerAdapter<ConfigurableRateLimiter> configurableRateLimiterLogger)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _configurableRateLimiterLogger = configurableRateLimiterLogger ?? throw new ArgumentNullException(nameof(configurableRateLimiterLogger));
        }

        /// <summary>
        /// Creates and configures an IRateLimiter instance based on the application settings.
        /// </summary>
        /// <returns>An IRateLimiter instance, specifically a ConfigurableRateLimiter.</returns>
        public IRateLimiter CreateRateLimiter()
        {
            _configurableRateLimiterLogger.Debug("RateLimiterFactory: Creating new ConfigurableRateLimiter instance.");
            // The ConfigurableRateLimiter is designed to manage all limits internally based on settings.
            // So the factory's role is simply to instantiate and return this main limiter,
            // passing along the necessary dependencies.
            return new ConfigurableRateLimiter(_settings, _configurableRateLimiterLogger);
        }
    }
}