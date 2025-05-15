using TheSSS.DICOMViewer.Monitoring.Interfaces;
using TheSSS.DICOMViewer.Monitoring.Contracts;
using TheSSS.DICOMViewer.Monitoring.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace TheSSS.DICOMViewer.Monitoring.Alerting.Strategies
{
    public class DefaultAlertDeduplicationStrategy : IAlertDeduplicationStrategy
    {
        private readonly DeduplicationOptions _deduplicationOptions;
        private readonly AlertingOptions _alertingOptions; // For Rule-specific overrides
        private readonly ILogger<DefaultAlertDeduplicationStrategy> _logger;
        private static readonly JsonSerializerOptions _jsonOptions = new() { DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull };


        // Key: Alert Signature (hash of defining content)
        // Value: Timestamp of last occurrence
        private readonly ConcurrentDictionary<string, DateTime> _processedAlertSignatures =
            new ConcurrentDictionary<string, DateTime>();

        public DefaultAlertDeduplicationStrategy(
            IOptions<AlertingOptions> alertingOptions,
            ILogger<DefaultAlertDeduplicationStrategy> logger)
        {
            _alertingOptions = alertingOptions?.Value ?? throw new ArgumentNullException(nameof(alertingOptions));
            _deduplicationOptions = _alertingOptions.Deduplication ?? throw new ArgumentNullException(nameof(alertingOptions.Value.Deduplication));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            // Consider a periodic cleanup task if the dictionary can grow very large and live for a long time.
            // For many scenarios, on-demand cleanup in IsDuplicateAsync is sufficient.
        }

        public Task<bool> IsDuplicateAsync(AlertContextDto alertContext, CancellationToken cancellationToken)
        {
            if (!_deduplicationOptions.IsEnabled)
            {
                _logger.LogTrace("Deduplication is disabled. Alert ID {AlertId} for rule '{RuleName}' will not be checked.", alertContext.AlertInstanceId, alertContext.TriggeredRuleName);
                return Task.FromResult(false);
            }

            var signature = GenerateAlertSignature(alertContext);
            var now = DateTime.UtcNow;
            var effectiveDeduplicationWindow = GetEffectiveDeduplicationWindow(alertContext);

            // Cleanup entries older than the largest possible window to prevent unbounded growth.
            // This is a simple cleanup; a dedicated timer could be more robust.
            CleanupOldEntries(now, effectiveDeduplicationWindow + TimeSpan.FromMinutes(5)); // Cleanup slightly beyond current window

            if (_processedAlertSignatures.TryGetValue(signature, out var lastProcessedTime))
            {
                if (now - lastProcessedTime < effectiveDeduplicationWindow)
                {
                    _logger.LogInformation("Alert ID {AlertId} for rule '{RuleName}' (Signature: {Signature}) is a DUPLICATE. Last processed at {LastTime}. Window: {Window}.",
                        alertContext.AlertInstanceId, alertContext.TriggeredRuleName, signature, lastProcessedTime, effectiveDeduplicationWindow);
                    // Optionally update the timestamp of the existing entry to "extend" its deduplication period
                    // _processedAlertSignatures[signature] = now;
                    return Task.FromResult(true);
                }
                _logger.LogDebug("Alert ID {AlertId} (Signature: {Signature}) found, but outside deduplication window. Last: {LastTime}, Now: {Now}, Window: {Window}.",
                    alertContext.AlertInstanceId, signature, lastProcessedTime, now, effectiveDeduplicationWindow);
            }
            
            _logger.LogDebug("Alert ID {AlertId} for rule '{RuleName}' (Signature: {Signature}) is NOT a duplicate.", alertContext.AlertInstanceId, alertContext.TriggeredRuleName, signature);
            return Task.FromResult(false);
        }

        public void RegisterProcessedAlert(AlertContextDto alertContext)
        {
            if (!_deduplicationOptions.IsEnabled) return;

            var signature = GenerateAlertSignature(alertContext);
            _processedAlertSignatures[signature] = DateTime.UtcNow;
            _logger.LogDebug("Registered alert ID {AlertId} (Signature: {Signature}) for deduplication tracking.", alertContext.AlertInstanceId, signature);
        }

        private TimeSpan GetEffectiveDeduplicationWindow(AlertContextDto alertContext)
        {
             var rule = _alertingOptions.Rules?.FirstOrDefault(r => r.RuleName == alertContext.TriggeredRuleName);
             return rule?.DeduplicationWindowOverride ?? _deduplicationOptions.DeduplicationWindow;
        }

        private string GenerateAlertSignature(AlertContextDto alertContext)
        {
            // Signature should be based on content that defines "sameness"
            // Excluding timestamp, alertInstanceId, etc.
            var sb = new StringBuilder();
            sb.Append(alertContext.TriggeredRuleName).Append('|');
            sb.Append(alertContext.SourceComponent).Append('|');
            sb.Append(alertContext.Severity.ToString()).Append('|');
            // A simplified message digest or key aspects of the message
            sb.Append(alertContext.Message.Length > 50 ? alertContext.Message.Substring(0, 50) : alertContext.Message).Append('|');

            // Include defining characteristics from RawData if available and relevant
            if (alertContext.RawData != null)
            {
                // Attempt to serialize RawData to a stable JSON string for hashing
                // This can be tricky if RawData has volatile fields (like timestamps within DTOs)
                // A more robust approach might be to extract specific key fields from RawData
                try
                {
                    // Example: if RawData is PacsConnectionInfoDto, include PacsNodeId and IsConnected status
                    if (alertContext.RawData is PacsConnectionInfoDto pacsInfo)
                    {
                        sb.Append($"PacsNodeId:{pacsInfo.PacsNodeId}|IsConnected:{pacsInfo.IsConnected}");
                    }
                    else if (alertContext.RawData is StorageHealthInfoDto storageInfo)
                    {
                        // For numeric data, consider bucketing or significant figures to avoid minor fluctuations creating new signatures
                        sb.Append($"StoragePath:{storageInfo.StoragePathIdentifier}|UsedPerc:{(int)(storageInfo.UsedPercentage / 5) * 5}"); // Bucket by 5%
                    }
                    else
                    {
                        // Generic serialization, might be too sensitive
                        // string rawDataJson = JsonSerializer.Serialize(alertContext.RawData, _jsonOptions);
                        // sb.Append(rawDataJson);
                        // Fallback: use type name if serialization is too complex/sensitive
                        sb.Append($"RawDataType:{alertContext.RawData.GetType().FullName}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to include RawData in alert signature generation for rule {RuleName}.", alertContext.TriggeredRuleName);
                    sb.Append("RawDataError");
                }
            }
            return GetSha256Hash(sb.ToString());
        }

        private string GetSha256Hash(string input)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(input);
                var hashBytes = sha256.ComputeHash(bytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
            }
        }

        private void CleanupOldEntries(DateTime now, TimeSpan maxWindow)
        {
            // Simple cleanup: iterate and remove old entries.
            // For very high alert volumes, this could be optimized or run on a separate timer.
            var keysToRemove = _processedAlertSignatures
                .Where(pair => (now - pair.Value) > maxWindow) // Remove entries older than the largest possible window
                .Select(pair => pair.Key)
                .ToList();

            if (keysToRemove.Any())
            {
                foreach (var key in keysToRemove)
                {
                    _processedAlertSignatures.TryRemove(key, out _);
                }
                _logger.LogDebug("Cleaned up {Count} old entries from deduplication cache.", keysToRemove.Count);
            }
        }
    }
}