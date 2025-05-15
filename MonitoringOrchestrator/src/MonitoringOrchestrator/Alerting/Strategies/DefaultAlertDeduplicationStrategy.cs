using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TheSSS.DICOMViewer.Monitoring.Configuration;
using TheSSS.DICOMViewer.Monitoring.Contracts;
using TheSSS.DICOMViewer.Monitoring.Interfaces;
using TheSSS.DICOMViewer.Monitoring.Interfaces.Adapters; // Assuming ILoggerAdapter

namespace TheSSS.DICOMViewer.Monitoring.Alerting.Strategies;

public class DefaultAlertDeduplicationStrategy : IAlertDeduplicationStrategy, IDisposable
{
    private readonly ILoggerAdapter<DefaultAlertDeduplicationStrategy> _logger;
    private readonly AlertingOptions _alertingOptions;

    // In-memory cache to track recent alert signatures for deduplication
    // Key: A hash or string signature representing the unique content of the alert
    // Value: The timestamp when this signature was last seen
    private readonly ConcurrentDictionary<string, DateTimeOffset> _recentAlerts = new();
    private readonly Timer _cleanupTimer; // Timer to periodically clean up old entries

    public DefaultAlertDeduplicationStrategy(ILoggerAdapter<DefaultAlertDeduplicationStrategy> logger, IOptions<AlertingOptions> alertingOptions)
    {
        _logger = logger;
        _alertingOptions = alertingOptions.Value;

        // Start cleanup timer - e.g., run every DeduplicationWindow or half of it
        var cleanupInterval = TimeSpan.FromMinutes(Math.Max(1, _alertingOptions.Deduplication.DeduplicationWindow.TotalMinutes / 2));
         // Ensure cleanupInterval is not Zero or negative
        if (cleanupInterval <= TimeSpan.Zero) cleanupInterval = TimeSpan.FromMinutes(1);

        _cleanupTimer = new Timer(CleanupRecentAlerts, null, TimeSpan.Zero, cleanupInterval);
    }

    /// <inheritdoc/>
    public Task<bool> IsDuplicateAsync(AlertContextDto alertContext, CancellationToken cancellationToken)
    {
        if (!_alertingOptions.Deduplication.IsEnabled)
        {
            _logger.Debug("Deduplication is disabled.");
            return Task.FromResult(false);
        }

        // Generate a signature for the current alert
        var alertSignature = GenerateAlertSignature(alertContext);
        var now = DateTimeOffset.UtcNow;

        // Check if the signature exists in the cache and is within the deduplication window
        if (_recentAlerts.TryGetValue(alertSignature, out var lastSeenTimestamp))
        {
            if ((now - lastSeenTimestamp) <= _alertingOptions.Deduplication.DeduplicationWindow)
            {
                 _logger.Debug($"Alert signature {alertSignature} found in cache within window {_alertingOptions.Deduplication.DeduplicationWindow}. Considered duplicate.");
                // Update timestamp to extend the deduplication window for this alert type
                _recentAlerts[alertSignature] = now;
                return Task.FromResult(true); // It's a duplicate
            }
            else
            {
                 _logger.Debug($"Alert signature {alertSignature} found, but outside window. Not a duplicate.");
                // Timestamp is outside the window, remove it (will be added back below)
                 _recentAlerts.TryRemove(alertSignature, out _);
            }
        }
         _logger.Debug($"Alert signature {alertSignature} not found or outside window. Not a duplicate. Adding to cache.");

        // Add or update the timestamp for the current alert signature
        _recentAlerts[alertSignature] = now; // Add or update

        return Task.FromResult(false); // Not a duplicate
    }

    private string GenerateAlertSignature(AlertContextDto alertContext)
    {
        // Create a string representation of the core alert details that define its uniqueness
        // This should be deterministic. Include RuleName, SourceComponent, Severity, and relevant parts of the message/RawData.
        // Hashing is a good way to get a fixed-size string representation. MD5 or SHA256 can be used.
        // Note: RawData can be complex objects; need a consistent way to serialize/represent its unique aspects.
        // For simplicity, let's hash a combination of RuleName, SourceComponent, Severity, and the Message.
        // If RawData needs to be part of uniqueness (e.g., specific PACS node failure), incorporate its key properties.

        var stringBuilder = new StringBuilder();
        stringBuilder.Append(alertContext.TriggeredRuleName);
        stringBuilder.Append('|');
        stringBuilder.Append(alertContext.SourceComponent);
        stringBuilder.Append('|');
        stringBuilder.Append(alertContext.AlertSeverity);
        stringBuilder.Append('|');
        stringBuilder.Append(alertContext.Message); // Message content can vary slightly, be cautious if this makes too many unique signatures

        // Example incorporating key properties from RawData if it's a known type
        if (alertContext.RawData is PacsConnectionInfoDto pacs)
        {
             stringBuilder.Append($"|PacsNode:{pacs.PacsNodeId}");
        }
         else if (alertContext.RawData is StorageHealthInfoDto storage)
        {
             stringBuilder.Append($"|StorageId:{storage.StorageIdentifier}");
             // Include percentage if relevant to uniqueness beyond just the rule name
             // stringBuilder.Append($"|UsedPerc:{storage.UsedPercentage:F1}");
        }
        // Add other specific RawData types...


        using (var sha256 = SHA256.Create())
        {
            byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(stringBuilder.ToString()));
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
        }
    }

    // Cleanup method for the timer
    private void CleanupRecentAlerts(object? state)
    {
        _logger.Debug("Running deduplication cache cleanup.");
        var now = DateTimeOffset.UtcNow;
        var deduplicationWindow = _alertingOptions.Deduplication.DeduplicationWindow;
        // If deduplicationWindow is zero or negative from config, use a sensible default for cleanup
        if (deduplicationWindow <= TimeSpan.Zero) deduplicationWindow = TimeSpan.FromMinutes(5);


        var keysToRemove = _recentAlerts
            .Where(pair => (now - pair.Value) > deduplicationWindow)
            .Select(pair => pair.Key)
            .ToList();

        foreach (var key in keysToRemove)
        {
            _recentAlerts.TryRemove(key, out _);
        }
        _logger.Debug($"Deduplication cache cleanup finished. Removed {keysToRemove.Count} entries. Current tracked entries: {_recentAlerts.Count}");
    }

    // Dispose the timer if the service is stopped
    public void Dispose()
    {
        _cleanupTimer?.Dispose();
        GC.SuppressFinalize(this);
    }
}