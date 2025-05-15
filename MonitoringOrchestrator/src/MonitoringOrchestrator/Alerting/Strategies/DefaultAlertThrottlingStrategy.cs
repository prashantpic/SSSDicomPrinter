using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TheSSS.DICOMViewer.Monitoring.Configuration;
using TheSSS.DICOMViewer.Monitoring.Contracts;
using TheSSS.DICOMViewer.Monitoring.Interfaces;
using TheSSS.DICOMViewer.Monitoring.Interfaces.Adapters; // Assuming ILoggerAdapter

namespace TheSSS.DICOMViewer.Monitoring.Alerting.Strategies;

public class DefaultAlertThrottlingStrategy : IAlertThrottlingStrategy, IDisposable
{
    private readonly ILoggerAdapter<DefaultAlertThrottlingStrategy> _logger;
    private readonly AlertingOptions _alertingOptions;

    // In-memory cache to track recent alerts for throttling
    // Key: A string uniquely identifying the alert type (e.g., RuleName + SourceComponent + InstanceIdentifier)
    // Value: A list of timestamps when the alert was sent
    private readonly ConcurrentDictionary<string, ConcurrentQueue<DateTimeOffset>> _alertTimestamps = new();
    private readonly Timer _cleanupTimer; // Timer to periodically clean up old timestamps

    public DefaultAlertThrottlingStrategy(ILoggerAdapter<DefaultAlertThrottlingStrategy> logger, IOptions<AlertingOptions> alertingOptions)
    {
        _logger = logger;
        _alertingOptions = alertingOptions.Value;

        // Start cleanup timer - e.g., run every hour or based on max window
        var cleanupInterval = TimeSpan.FromMinutes(Math.Max(1, _alertingOptions.Throttling.DefaultThrottleWindow.TotalMinutes / 2)); // Cleanup every half the window
        // Ensure cleanupInterval is not Zero or negative
        if (cleanupInterval <= TimeSpan.Zero) cleanupInterval = TimeSpan.FromMinutes(1);

        _cleanupTimer = new Timer(CleanupAlertTimestamps, null, TimeSpan.Zero, cleanupInterval);
    }

    /// <inheritdoc/>
    public Task<bool> ShouldThrottleAsync(AlertContextDto alertContext, CancellationToken cancellationToken)
    {
        if (!_alertingOptions.Throttling.IsEnabled)
        {
            _logger.Debug("Throttling is disabled.");
            return Task.FromResult(false);
        }

        // Generate a unique key for this alert type
        // Consider adding InstanceIdentifier if the rule uses it
        var alertKey = GetAlertKey(alertContext);
        var now = DateTimeOffset.UtcNow;

        // Get or create the queue for this alert key
        var timestamps = _alertTimestamps.GetOrAdd(alertKey, _ => new ConcurrentQueue<DateTimeOffset>());

        // Define the throttling window for this alert (could be rule-specific)
        var throttleWindow = _alertingOptions.Throttling.DefaultThrottleWindow;
        var maxAlerts = _alertingOptions.Throttling.MaxAlertsPerWindow;

        // Remove timestamps outside the current window
        while (timestamps.TryPeek(out var firstTimestamp) && (now - firstTimestamp) > throttleWindow)
        {
            timestamps.TryDequeue(out _);
        }

        // Check if adding the current alert would exceed the max allowed in the window
        if (timestamps.Count >= maxAlerts)
        {
            _logger.Debug($"Throttling alert '{alertContext.TriggeredRuleName}': {timestamps.Count} alerts in window {throttleWindow}, max is {maxAlerts}.");
            return Task.FromResult(true); // Throttle the alert
        }

        // Add the current timestamp to the queue
        timestamps.Enqueue(now);
         _logger.Debug($"Not throttling alert '{alertContext.TriggeredRuleName}'. Count in window: {timestamps.Count}.");
        return Task.FromResult(false); // Do not throttle
    }

    private string GetAlertKey(AlertContextDto alertContext)
    {
        // Creates a unique key based on rule name and source.
        // Add alertContext.RawData type or properties if different instances of the same rule
        // (e.g., "PACS_Offline" for different AE Titles) should be throttled independently.
        // Example including InstanceIdentifier if applicable:
        return $"{alertContext.TriggeredRuleName}|{alertContext.SourceComponent}|{GetRawDataInstanceIdentifier(alertContext.RawData)}";
    }

    // Helper to get instance identifier from raw data (e.g., PacsNodeId)
    private string GetRawDataInstanceIdentifier(object? rawData)
    {
        return rawData switch
        {
            PacsConnectionInfoDto pacs => pacs.PacsNodeId,
            StorageHealthInfoDto storage => storage.StorageIdentifier ?? "DefaultStorage",
            // Add cases for other types that might have instance identifiers
            _ => "" // Default if no specific identifier is found/needed
        };
    }


    // Cleanup method for the timer
    private void CleanupAlertTimestamps(object? state)
    {
        _logger.Debug("Running throttling cache cleanup.");
        var now = DateTimeOffset.UtcNow;
        var throttleWindow = _alertingOptions.Throttling.DefaultThrottleWindow; // Use default window for cleanup
         // If throttleWindow is zero or negative from config, use a sensible default for cleanup
        if (throttleWindow <= TimeSpan.Zero) throttleWindow = TimeSpan.FromHours(1);


        foreach (var pair in _alertTimestamps)
        {
            var queue = pair.Value;
            while (queue.TryPeek(out var timestamp) && (now - timestamp) > throttleWindow)
            {
                queue.TryDequeue(out _);
            }
            // Remove empty queues to prevent dictionary from growing indefinitely
            if (queue.IsEmpty)
            {
                _alertTimestamps.TryRemove(pair.Key, out _);
            }
        }
         _logger.Debug($"Throttling cache cleanup finished. Current tracked alert types: {_alertTimestamps.Count}");
    }

    // Dispose the timer if the service is stopped
    public void Dispose()
    {
        _cleanupTimer?.Dispose();
        GC.SuppressFinalize(this);
    }
}