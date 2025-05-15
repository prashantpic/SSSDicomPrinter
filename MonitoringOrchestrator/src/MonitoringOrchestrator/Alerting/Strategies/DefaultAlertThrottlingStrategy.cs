using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TheSSS.DICOMViewer.Monitoring.Configuration;
using TheSSS.DICOMViewer.Monitoring.Contracts;
using TheSSS.DICOMViewer.Monitoring.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TheSSS.DICOMViewer.Monitoring.Alerting.Strategies
{
    /// <summary>
    /// Default implementation for alert throttling based on time windows and frequency.
    /// Implements a basic time-window based alert throttling mechanism.
    /// </summary>
    public class DefaultAlertThrottlingStrategy : IAlertThrottlingStrategy
    {
        private readonly IOptions<AlertingOptions> _alertingOptions;
        private readonly ILogger<DefaultAlertThrottlingStrategy> _logger;
        
        // Key: Alert Signature (e.g., RuleName + SourceComponent + optional TargetIdentifier from AlertRule)
        // Value: List of timestamps when this alert was last fired.
        private readonly ConcurrentDictionary<string, List<DateTimeOffset>> _alertFireTimestamps;
        private readonly Timer _cleanupTimer;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultAlertThrottlingStrategy"/> class.
        /// </summary>
        /// <param name="alertingOptions">The alerting configuration options.</param>
        /// <param name="logger">The logger.</param>
        public DefaultAlertThrottlingStrategy(
            IOptions<AlertingOptions> alertingOptions,
            ILogger<DefaultAlertThrottlingStrategy> logger)
        {
            _alertingOptions = alertingOptions ?? throw new ArgumentNullException(nameof(alertingOptions));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _alertFireTimestamps = new ConcurrentDictionary<string, List<DateTimeOffset>>();

            // Cleanup old entries periodically to prevent memory leaks.
            // The cleanup interval could be configurable. For now, set to a fraction of typical throttle windows.
            var cleanupInterval = TimeSpan.FromMinutes(Math.Max(1, _alertingOptions.Value.Throttling.DefaultThrottleWindow.TotalMinutes / 4));
            _cleanupTimer = new Timer(CleanupOldEntries, null, cleanupInterval, cleanupInterval);
        }

        /// <inheritdoc/>
        public Task<bool> ShouldThrottleAsync(AlertContextDto alertContext, CancellationToken cancellationToken)
        {
            var throttlingOptions = _alertingOptions.Value.Throttling;
            if (!throttlingOptions.IsEnabled)
            {
                return Task.FromResult(false);
            }

            // A more specific alert signature could include TargetIdentifier if the rule is specific.
            // For now, using RuleName and SourceComponent as a general key.
            // AlertContextDto.AlertHash could also be used if it's designed for throttling signature.
            // Let's use TriggeredRuleName + SourceComponent as the key for throttling.
            string alertSignature = $"{alertContext.TriggeredRuleName}_{alertContext.SourceComponent}";

            var now = DateTimeOffset.UtcNow;
            var throttleWindow = throttlingOptions.DefaultThrottleWindow;
            var maxAlertsPerWindow = throttlingOptions.MaxAlertsPerWindow;

            var timestamps = _alertFireTimestamps.GetOrAdd(alertSignature, _ => new List<DateTimeOffset>());

            lock (timestamps) // Ensure thread-safe access to the list for this specific signature
            {
                // Remove timestamps older than the throttle window
                timestamps.RemoveAll(ts => (now - ts) > throttleWindow);

                if (timestamps.Count >= maxAlertsPerWindow)
                {
                    _logger.LogInformation("Alert '{AlertSignature}' throttled. Fired {Count} times in the last {Window}.",
                        alertSignature, timestamps.Count, throttleWindow);
                    return Task.FromResult(true); // Throttle
                }

                // Add current timestamp and allow alert
                timestamps.Add(now);
                _logger.LogDebug("Alert '{AlertSignature}' not throttled. Fire count: {Count}/{MaxCount} in window.",
                    alertSignature, timestamps.Count, maxAlertsPerWindow);
                return Task.FromResult(false); // Do not throttle
            }
        }

        private void CleanupOldEntries(object? state)
        {
            var throttlingOptions = _alertingOptions.Value.Throttling;
            if (!throttlingOptions.IsEnabled) return;

            var now = DateTimeOffset.UtcNow;
            var throttleWindow = throttlingOptions.DefaultThrottleWindow;
            
            _logger.LogDebug("Running cleanup for alert throttling strategy cache.");

            foreach (var key in _alertFireTimestamps.Keys.ToList()) // ToList to avoid modification issues during iteration
            {
                if (_alertFireTimestamps.TryGetValue(key, out var timestamps))
                {
                    lock (timestamps)
                    {
                        timestamps.RemoveAll(ts => (now - ts) > throttleWindow.Add(TimeSpan.FromMinutes(5))); // Add a buffer
                        if (!timestamps.Any())
                        {
                            _alertFireTimestamps.TryRemove(key, out _);
                            _logger.LogDebug("Removed empty timestamp list for alert signature '{AlertSignature}'.", key);
                        }
                    }
                }
            }
            _logger.LogDebug("Finished cleanup for alert throttling strategy cache.");
        }
        
        // Dispose the timer when the strategy is disposed (if it were disposable)
        // For a singleton, this might be managed by the application lifecycle if using IDisposable.
        // For simplicity here, BackgroundService would typically handle this kind of cleanup on shutdown.
        // If this strategy is registered as a singleton, consider IDisposable.
    }
}