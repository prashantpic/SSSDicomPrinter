using TheSSS.DICOMViewer.Monitoring.Interfaces;
using TheSSS.DICOMViewer.Monitoring.Contracts;
using TheSSS.DICOMViewer.Monitoring.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TheSSS.DICOMViewer.Monitoring.Alerting.Strategies
{
    public class DefaultAlertThrottlingStrategy : IAlertThrottlingStrategy
    {
        private readonly AlertingOptions _alertingOptions;
        private readonly ILogger<DefaultAlertThrottlingStrategy> _logger;

        // Key: Alert Key (RuleName + SourceComponent + SubMetric if applicable)
        // Value: List of timestamps of when alerts were allowed through for this key
        private readonly ConcurrentDictionary<string, ConcurrentQueue<DateTime>> _alertTimestamps =
            new ConcurrentDictionary<string, ConcurrentQueue<DateTime>>();

        public DefaultAlertThrottlingStrategy(
            IOptions<AlertingOptions> alertingOptions,
            ILogger<DefaultAlertThrottlingStrategy> logger)
        {
            _alertingOptions = alertingOptions?.Value ?? throw new ArgumentNullException(nameof(alertingOptions));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task<bool> ShouldThrottleAsync(AlertContextDto alertContext, CancellationToken cancellationToken)
        {
            if (!_alertingOptions.Throttling.IsEnabled)
            {
                _logger.LogTrace("Throttling is disabled. Alert ID {AlertId} for rule '{RuleName}' will not be throttled.", alertContext.AlertInstanceId, alertContext.TriggeredRuleName);
                return Task.FromResult(false);
            }

            var alertKey = GetAlertKey(alertContext);
            var now = DateTime.UtcNow;
            var throttleWindow = GetEffectiveThrottleWindow(alertContext);
            var maxAlertsInWindow = _alertingOptions.Throttling.MaxAlertsPerWindow;

            if (maxAlertsInWindow <= 0) // If max alerts is 0 or negative, effectively no throttling by count
            {
                _logger.LogTrace("MaxAlertsPerWindow is {MaxCount}, effectively disabling count-based throttling for alert ID {AlertId}, rule '{RuleName}'.", maxAlertsInWindow, alertContext.AlertInstanceId, alertContext.TriggeredRuleName);
                // Still, we might register this alert to enable time-based throttling if window is > 0
                 if (throttleWindow > TimeSpan.Zero)
                 {
                    var queue = _alertTimestamps.GetOrAdd(alertKey, _ => new ConcurrentQueue<DateTime>());
                    queue.Enqueue(now); // Add current alert time
                    // Prune old timestamps (older than window)
                    while (queue.TryPeek(out var oldest) && (now - oldest >= throttleWindow))
                    {
                        queue.TryDequeue(out _);
                    }
                 }
                return Task.FromResult(false);
            }
            
            var alertQueue = _alertTimestamps.GetOrAdd(alertKey, _ => new ConcurrentQueue<DateTime>());

            // Prune old timestamps (older than window)
            while (alertQueue.TryPeek(out var oldest) && (now - oldest >= throttleWindow))
            {
                alertQueue.TryDequeue(out _);
            }

            // Check if count within window exceeds max
            if (alertQueue.Count >= maxAlertsInWindow)
            {
                _logger.LogInformation("Alert ID {AlertId} for rule '{RuleName}' (Key: {AlertKey}) is THROTTLED. Count in window ({Count}) >= MaxAlertsPerWindow ({MaxAlerts}). Window: {ThrottleWindow}.",
                    alertContext.AlertInstanceId, alertContext.TriggeredRuleName, alertKey, alertQueue.Count, maxAlertsInWindow, throttleWindow);
                return Task.FromResult(true);
            }

            // If not throttled, add current timestamp and allow
            alertQueue.Enqueue(now);
            _logger.LogDebug("Alert ID {AlertId} for rule '{RuleName}' (Key: {AlertKey}) is NOT throttled. Count in window: {Count}. Adding current timestamp.",
                 alertContext.AlertInstanceId, alertContext.TriggeredRuleName, alertKey, alertQueue.Count);

            return Task.FromResult(false);
        }

        private TimeSpan GetEffectiveThrottleWindow(AlertContextDto alertContext)
        {
            var rule = _alertingOptions.Rules?.FirstOrDefault(r => r.RuleName == alertContext.TriggeredRuleName);
            return rule?.ThrottleWindowOverride ?? _alertingOptions.Throttling.DefaultThrottleWindow;
        }

        private string GetAlertKey(AlertContextDto alertContext)
        {
            // This key should be specific enough to group similar alerts for throttling purposes.
            // Using RuleName and SourceComponent is a good start.
            // If RawData contains specific identifiers (e.g., PACS AETitle, Task Name), include them.
            string subIdentifier = "GLOBAL"; // Default if no specific sub-identifier

            if (alertContext.RawData is PacsConnectionInfoDto pacsInfo)
            {
                subIdentifier = pacsInfo.PacsNodeId ?? "UNKNOWN_PACS";
            }
            else if (alertContext.RawData is AutomatedTaskStatusInfoDto taskInfo)
            {
                subIdentifier = taskInfo.TaskName ?? "UNKNOWN_TASK";
            }
            // Add other specific DTO checks as needed.

            return $"{alertContext.TriggeredRuleName}_{alertContext.SourceComponent}_{subIdentifier}";
        }
    }
}