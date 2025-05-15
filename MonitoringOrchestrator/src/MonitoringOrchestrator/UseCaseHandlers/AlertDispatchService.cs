namespace TheSSS.DICOMViewer.Monitoring.UseCaseHandlers;

using TheSSS.DICOMViewer.Monitoring.Contracts;
using TheSSS.DICOMViewer.Monitoring.Interfaces;
using TheSSS.DICOMViewer.Monitoring.Interfaces.Adapters;
using TheSSS.DICOMViewer.Monitoring.Configuration;
using TheSSS.DICOMViewer.Monitoring.Mappers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class AlertDispatchService
{
    private readonly IEnumerable<IAlertingChannel> _alertingChannels;
    private readonly IAlertThrottlingStrategy _throttlingStrategy;
    private readonly IAlertDeduplicationStrategy _deduplicationStrategy;
    private readonly AlertingOptions _alertingOptions;
    private readonly IAuditLoggingAdapter _auditLoggingAdapter;
    private readonly ILogger<AlertDispatchService> _logger;

    public AlertDispatchService(
        IEnumerable<IAlertingChannel> alertingChannels,
        IAlertThrottlingStrategy throttlingStrategy,
        IAlertDeduplicationStrategy deduplicationStrategy,
        IOptions<AlertingOptions> alertingOptions,
        IAuditLoggingAdapter auditLoggingAdapter,
        ILogger<AlertDispatchService> logger)
    {
        _alertingChannels = alertingChannels ?? throw new ArgumentNullException(nameof(alertingChannels));
        _throttlingStrategy = throttlingStrategy ?? throw new ArgumentNullException(nameof(throttlingStrategy));
        _deduplicationStrategy = deduplicationStrategy ?? throw new ArgumentNullException(nameof(deduplicationStrategy));
        _alertingOptions = alertingOptions?.Value ?? throw new ArgumentNullException(nameof(alertingOptions));
        _auditLoggingAdapter = auditLoggingAdapter ?? throw new ArgumentNullException(nameof(auditLoggingAdapter));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Evaluates throttling and deduplication for an alert and dispatches it through configured channels.
    /// </summary>
    /// <param name="alertContext">The context of the alert to dispatch.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task DispatchAlertAsync(AlertContextDto alertContext, CancellationToken cancellationToken)
    {
        _logger.LogDebug($"Attempting to dispatch alert '{alertContext.AlertInstanceId}' for rule '{alertContext.TriggeredRuleName}'.");

        // --- 1. Deduplication Check ---
        if (_alertingOptions.Deduplication.IsEnabled)
        {
            if (await _deduplicationStrategy.IsDuplicateAsync(alertContext, cancellationToken))
            {
                _logger.LogInformation($"Alert '{alertContext.AlertInstanceId}' (Rule: '{alertContext.TriggeredRuleName}') is a duplicate. Skipping dispatch.");
                await LogSkippedAlertAsync("SystemAlertSkippedDuplicate", alertContext, "Skipped (Duplicate)");
                return;
            }
        }

        // --- 2. Throttling Check ---
        // Note: Throttling strategy might update its state internally if it decides NOT to throttle.
        if (_alertingOptions.Throttling.IsEnabled)
        {
            if (await _throttlingStrategy.ShouldThrottleAsync(alertContext, cancellationToken))
            {
                _logger.LogInformation($"Alert '{alertContext.AlertInstanceId}' (Rule: '{alertContext.TriggeredRuleName}') is throttled. Skipping dispatch.");
                await LogSkippedAlertAsync("SystemAlertSkippedThrottled", alertContext, "Skipped (Throttled)");
                return;
            }
        }
        
        // If not duplicate and not throttled, proceed to register and dispatch.
        // Register *after* throttling check, but *before* dispatching to channels.
        // This ensures that if throttling allows it, it's counted for deduplication (if dedupe didn't catch it first).
        if (_alertingOptions.Deduplication.IsEnabled)
        {
            _deduplicationStrategy.RegisterProcessedAlert(alertContext);
        }


        _logger.LogInformation($"Dispatching alert '{alertContext.AlertInstanceId}' (Rule: '{alertContext.TriggeredRuleName}') to configured channels.");
        await _auditLoggingAdapter.LogAuditEventAsync(
            "SystemAlertDispatchInitiated",
            $"Dispatching alert. Rule: {alertContext.TriggeredRuleName}, Severity: {alertContext.Severity}, Message: {alertContext.Message}",
            "Initiated",
            alertContext.SourceComponent ?? _alertingOptions.DefaultAlertSourceComponent);

        var dispatchTasks = new List<Task>();

        foreach (var channelSetting in _alertingOptions.Channels.Where(c => c.IsEnabled))
        {
            if (cancellationToken.IsCancellationRequested) break;

            // Check if alert severity is allowed for this channel
            if (channelSetting.Severities != null && channelSetting.Severities.Any() &&
                !channelSetting.Severities.Any(s => s.Equals(alertContext.Severity.ToString(), StringComparison.OrdinalIgnoreCase)))
            {
                _logger.LogDebug($"Alert severity '{alertContext.Severity}' is not configured for channel '{channelSetting.ChannelType}'. Skipping channel for this alert.");
                continue;
            }

            var channel = _alertingChannels.FirstOrDefault(c => c.ChannelType.Equals(channelSetting.ChannelType, StringComparison.OrdinalIgnoreCase));

            if (channel != null)
            {
                // Add dispatch to a list of tasks to run them concurrently
                dispatchTasks.Add(DispatchToChannelAsync(channel, alertContext, channelSetting, cancellationToken));
            }
            else
            {
                _logger.LogWarning($"No IAlertingChannel implementation found for configured channel type: '{channelSetting.ChannelType}'.");
                await _auditLoggingAdapter.LogAuditEventAsync(
                   "SystemAlertConfigError",
                   $"No implementation found for channel type: {channelSetting.ChannelType} for alert '{alertContext.AlertInstanceId}'",
                   "ConfigurationError",
                   alertContext.SourceComponent ?? _alertingOptions.DefaultAlertSourceComponent);
            }
        }

        await Task.WhenAll(dispatchTasks); // Wait for all channel dispatches to complete or fail

        _logger.LogDebug($"Finished attempting to dispatch alert '{alertContext.AlertInstanceId}'.");
    }

    private async Task DispatchToChannelAsync(IAlertingChannel channel, AlertContextDto alertContext, AlertChannelSetting channelSetting, CancellationToken cancellationToken)
    {
        try
        {
            var payload = HealthReportMapper.ToNotificationPayload(
                alertContext,
                channel.ChannelType,
                channelSetting.RecipientDetails,
                _alertingOptions.DefaultAlertSourceComponent);

            _logger.LogDebug($"Dispatching alert '{alertContext.AlertInstanceId}' to channel: {channel.ChannelType}");
            await channel.DispatchAlertAsync(payload, cancellationToken);
            _logger.LogInformation($"Successfully dispatched alert '{alertContext.AlertInstanceId}' to channel: {channel.ChannelType}");

            // Audit successful dispatch (if not handled by AuditLogAlertingChannel itself)
             if (channel.ChannelType != "AuditLog") // Avoid double logging if AuditLog is a channel
             {
                 await _auditLoggingAdapter.LogAuditEventAsync(
                    "SystemAlertDispatchedToChannel",
                    $"Alert successfully sent via {channel.ChannelType}. Rule: {alertContext.TriggeredRuleName}, Severity: {alertContext.Severity}",
                    "Success",
                    alertContext.SourceComponent ?? _alertingOptions.DefaultAlertSourceComponent);
             }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to dispatch alert '{alertContext.AlertInstanceId}' to channel '{channel.ChannelType}'.");
            await _auditLoggingAdapter.LogAuditEventAsync(
                "SystemAlertDispatchChannelFailure",
                $"Failed to dispatch alert to {channel.ChannelType}. Rule: {alertContext.TriggeredRuleName}, Error: {ex.Message}",
                "Failure",
                alertContext.SourceComponent ?? _alertingOptions.DefaultAlertSourceComponent);
            // Do not re-throw, allow other channels to attempt dispatch
        }
    }
    
    private async Task LogSkippedAlertAsync(string eventType, AlertContextDto alertContext, string outcome)
    {
        await _auditLoggingAdapter.LogAuditEventAsync(
            eventType,
            $"Alert skipped. Rule: {alertContext.TriggeredRuleName}, Severity: {alertContext.Severity}, Message: {alertContext.Message}, InstanceId: {alertContext.AlertInstanceId}",
            outcome,
            alertContext.SourceComponent ?? _alertingOptions.DefaultAlertSourceComponent);
    }
}