using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TheSSS.DICOMViewer.Monitoring.Configuration;
using TheSSS.DICOMViewer.Monitoring.Contracts;
using TheSSS.DICOMViewer.Monitoring.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TheSSS.DICOMViewer.Monitoring.Alerting.Strategies
{
    /// <summary>
    /// Default implementation for alert deduplication based on recent identical alerts.
    /// Implements a basic content-based alert deduplication mechanism.
    /// </summary>
    public class DefaultAlertDeduplicationStrategy : IAlertDeduplicationStrategy
    {
        private readonly IOptions<AlertingOptions> _alertingOptions;
        private readonly ILogger<DefaultAlertDeduplicationStrategy> _logger;

        // Key: Alert Hash (from AlertContextDto.AlertHash)
        // Value: Timestamp of the last occurrence.
        private readonly ConcurrentDictionary<string, DateTimeOffset> _recentAlertHashes;
        private readonly Timer _cleanupTimer;


        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultAlertDeduplicationStrategy"/> class.
        /// </summary>
        /// <param name="alertingOptions">The alerting configuration options.</param>
        /// <param name="logger">The logger.</param>
        public DefaultAlertDeduplicationStrategy(
            IOptions<AlertingOptions> alertingOptions,
            ILogger<DefaultAlertDeduplicationStrategy> logger)
        {
            _alertingOptions = alertingOptions ?? throw new ArgumentNullException(nameof(alertingOptions));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _recentAlertHashes = new ConcurrentDictionary<string, DateTimeOffset>();
            
            var cleanupInterval = TimeSpan.FromMinutes(Math.Max(1, _alertingOptions.Value.Deduplication.DeduplicationWindow.TotalMinutes / 2));
            _cleanupTimer = new Timer(CleanupOldEntries, null, cleanupInterval, cleanupInterval);
        }

        /// <inheritdoc/>
        public Task<bool> IsDuplicateAsync(AlertContextDto alertContext, CancellationToken cancellationToken)
        {
            var deduplicationOptions = _alertingOptions.Value.Deduplication;
            if (!deduplicationOptions.IsEnabled)
            {
                return Task.FromResult(false);
            }

            if (string.IsNullOrEmpty(alertContext.AlertHash))
            {
                _logger.LogWarning("AlertHash is missing in AlertContextDto for rule '{RuleName}'. Cannot perform deduplication.", alertContext.TriggeredRuleName);
                return Task.FromResult(false); // Cannot deduplicate without a hash
            }

            var now = DateTimeOffset.UtcNow;
            var deduplicationWindow = deduplicationOptions.DeduplicationWindow;

            if (_recentAlertHashes.TryGetValue(alertContext.AlertHash, out var lastOccurrenceTimestamp))
            {
                if ((now - lastOccurrenceTimestamp) <= deduplicationWindow)
                {
                    _logger.LogInformation("Alert with hash '{AlertHash}' (Rule: '{RuleName}') is a duplicate within the {Window} window. Suppressing.",
                        alertContext.AlertHash, alertContext.TriggeredRuleName, deduplicationWindow);
                    // Optionally update timestamp to extend suppression if desired (e.g. keep seeing it extends its suppression window)
                    // _recentAlertHashes[alertContext.AlertHash] = now; 
                    return Task.FromResult(true); // It's a duplicate
                }
            }

            // Not a duplicate, or outside the window. Record/update it.
            _recentAlertHashes[alertContext.AlertHash] = now;
            _logger.LogDebug("Alert with hash '{AlertHash}' (Rule: '{RuleName}') is not a duplicate or its last occurrence is outside the window. Processing.",
                alertContext.AlertHash, alertContext.TriggeredRuleName);
            return Task.FromResult(false); // Not a duplicate
        }
        
        private void CleanupOldEntries(object? state)
        {
            var deduplicationOptions = _alertingOptions.Value.Deduplication;
            if (!deduplicationOptions.IsEnabled) return;

            var now = DateTimeOffset.UtcNow;
            var deduplicationWindow = deduplicationOptions.DeduplicationWindow;

            _logger.LogDebug("Running cleanup for alert deduplication strategy cache.");

            var keysToRemove = _recentAlertHashes
                .Where(pair => (now - pair.Value) > deduplicationWindow.Add(TimeSpan.FromMinutes(1))) // Add a buffer
                .Select(pair => pair.Key)
                .ToList();

            foreach (var key in keysToRemove)
            {
                if (_recentAlertHashes.TryRemove(key, out _))
                {
                    _logger.LogDebug("Removed old alert hash '{AlertHash}' from deduplication cache.", key);
                }
            }
            _logger.LogDebug("Finished cleanup for alert deduplication strategy cache.");
        }
    }
}