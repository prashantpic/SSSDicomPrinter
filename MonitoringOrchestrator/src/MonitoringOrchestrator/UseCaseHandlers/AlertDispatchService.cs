using Microsoft.Extensions.Options;
using TheSSS.DICOMViewer.Monitoring.Configuration;
using TheSSS.DICOMViewer.Monitoring.Contracts;
using TheSSS.DICOMViewer.Monitoring.Exceptions;
using TheSSS.DICOMViewer.Monitoring.Interfaces;
using TheSSS.DICOMViewer.Monitoring.Interfaces.Adapters;
using TheSSS.DICOMViewer.Monitoring.Mappers;

namespace TheSSS.DICOMViewer.Monitoring.UseCaseHandlers;

public class AlertDispatchService
{
    private readonly ILoggerAdapter<AlertDispatchService> _logger;
    private readonly AlertingOptions _alertingOptions;
    private readonly IAlertThrottlingStrategy _throttlingStrategy;
    private readonly IAlertDeduplicationStrategy _deduplicationStrategy;
    private readonly IEnumerable<IAlertingChannel> _alertingChannels;
    private readonly IAuditLoggingAdapter _auditLoggingAdapter;
    private readonly HealthReportMapper _mapper;

    public AlertDispatchService(
        ILoggerAdapter<AlertDispatchService> logger,
        IOptions<AlertingOptions> alertingOptions,
        IAlertThrottlingStrategy throttlingStrategy,
        IAlertDeduplicationStrategy deduplicationStrategy,
        IEnumerable<IAlertingChannel> alertingChannels,
        IAuditLoggingAdapter auditLoggingAdapter,
        HealthReportMapper mapper)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _alertingOptions = alertingOptions?.Value ?? throw new ArgumentNullException(nameof(alertingOptions));
        _throttlingStrategy = throttlingStrategy ?? throw new ArgumentNullException(nameof(throttlingStrategy));
        _deduplicationStrategy = deduplicationStrategy ?? throw new ArgumentNullException(nameof(deduplicationStrategy));
        _alertingChannels = alertingChannels ?? throw new ArgumentNullException(nameof(alertingChannels));
        _auditLoggingAdapter = auditLoggingAdapter ?? throw new ArgumentNullException(nameof(auditLoggingAdapter));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task DispatchAlertAsync(AlertContextDto alertContext, CancellationToken cancellationToken)
    {
        if (alertContext == null)
        {
            _logger.Error("DispatchAlertAsync called with null alertContext.");
            throw new ArgumentNullException(nameof(alertContext));
        }

        _logger.Info($"Received alert for rule '{alertContext.TriggeredRuleName}', severity '{alertContext.AlertSeverity}'. Evaluating dispatch.");

        if (!_alertingOptions.IsEnabled)
        {
            _logger.Info("Alerting is globally disabled. Alert not dispatched.");
            await _auditLoggingAdapter.LogAuditEventAsync(
                eventType: "AlertDispatchSkipped",
                eventDetails: $"Rule: {alertContext.TriggeredRuleName}, Message: {alertContext.Message}. Reason: Alerting globally disabled.",
                outcome: "Skipped",
                sourceComponent: nameof(AlertDispatchService));
            return;
        }

        // 1. Deduplication
        if (_alertingOptions.Deduplication.IsEnabled)
        {
            if (await _deduplicationStrategy.IsDuplicateAsync(alertContext, cancellationToken))
            {
                _logger.Info($"Alert for rule '{alertContext.TriggeredRuleName}' (Severity: {alertContext.AlertSeverity}) is a duplicate. Skipping dispatch.");
                await _auditLoggingAdapter.LogAuditEventAsync(
                    eventType: "AlertDeduplicated",
                    eventDetails: $"Rule: {alertContext.TriggeredRuleName}, Severity: {alertContext.AlertSeverity}, Message: {alertContext.Message}",
                    outcome: "Skipped",
                    sourceComponent: nameof(AlertDispatchService));
                return;
            }
            _logger.Debug($"Alert for rule '{alertContext.TriggeredRuleName}' not a duplicate.");
        }

        // 2. Throttling
        if (_alertingOptions.Throttling.IsEnabled)
        {
            if (await _throttlingStrategy.ShouldThrottleAsync(alertContext, cancellationToken))
            {
                _logger.Info($"Alert for rule '{alertContext.TriggeredRuleName}' (Severity: {alertContext.AlertSeverity}) is throttled. Skipping dispatch.");
                await _auditLoggingAdapter.LogAuditEventAsync(
                    eventType: "AlertThrottled",
                    eventDetails: $"Rule: {alertContext.TriggeredRuleName}, Severity: {alertContext.AlertSeverity}, Message: {alertContext.Message}",
                    outcome: "Skipped",
                    sourceComponent: nameof(AlertDispatchService));
                return;
            }
            _logger.Debug($"Alert for rule '{alertContext.TriggeredRuleName}' not throttled.");
        }

        _logger.Info($"Proceeding to dispatch alert for rule '{alertContext.TriggeredRuleName}', severity '{alertContext.AlertSeverity}'.");

        var enabledChannelsSettings = _alertingOptions.Channels?
            .Where(c => c.IsEnabled && IsSeveritySufficient(alertContext.AlertSeverity, c.MinimumSeverity))
            .ToList() ?? new List<AlertChannelSetting>();

        if (!enabledChannelsSettings.Any())
        {
            _logger.Warning($"No enabled alerting channels configured for severity '{alertContext.AlertSeverity}' or at all. Alert for rule '{alertContext.TriggeredRuleName}' not dispatched.");
            await _auditLoggingAdapter.LogAuditEventAsync(
               eventType: "AlertDispatchFailed",
               eventDetails: $"Rule: {alertContext.TriggeredRuleName}, Message: {alertContext.Message}. Reason: No enabled channels for severity '{alertContext.AlertSeverity}'.",
               outcome: "Failure",
               sourceComponent: nameof(AlertDispatchService));
            return;
        }

        var notificationPayload = _mapper.ToNotificationPayload(alertContext);
        var dispatchTasks = new List<Task>();

        foreach (var channelSetting in enabledChannelsSettings)
        {
            var channelImplementation = _alertingChannels.FirstOrDefault(
                impl => impl.GetType().Name.Equals(channelSetting.ChannelType + "AlertingChannel", StringComparison.OrdinalIgnoreCase));

            if (channelImplementation != null)
            {
                _logger.Debug($"Preparing to dispatch alert via channel: {channelSetting.ChannelType}.");
                
                // Clone or create a new payload to avoid shared state issues if channels modify it, though they shouldn't.
                var channelSpecificPayload = new NotificationPayloadDto
                {
                    Title = notificationPayload.Title,
                    Body = notificationPayload.Body,
                    Severity = notificationPayload.Severity,
                    Timestamp = notificationPayload.Timestamp,
                    CorrelationId = notificationPayload.CorrelationId,
                    TargetChannelType = channelSetting.ChannelType,
                };

                if (channelSetting.ChannelType.Equals("Email", StringComparison.OrdinalIgnoreCase))
                {
                    channelSpecificPayload.RecipientDetails = channelSetting.RecipientEmailAddresses;
                }
                // Add other channel-specific recipient details if necessary

                dispatchTasks.Add(DispatchToChannelAsync(channelImplementation, channelSpecificPayload, alertContext, cancellationToken));
            }
            else
            {
                _logger.Warning($"No implementation found for configured and enabled channel type: {channelSetting.ChannelType}. Rule: '{alertContext.TriggeredRuleName}'.");
                await _auditLoggingAdapter.LogAuditEventAsync(
                   eventType: "AlertDispatchFailed",
                   eventDetails: $"Rule: {alertContext.TriggeredRuleName}. Reason: No implementation for channel type '{channelSetting.ChannelType}'.",
                   outcome: "ConfigurationError",
                   sourceComponent: nameof(AlertDispatchService));
            }
        }

        await Task.WhenAll(dispatchTasks);
        _logger.Info($"Alert dispatch process completed for rule '{alertContext.TriggeredRuleName}'. Dispatched to {dispatchTasks.Count} channels.");
    }

    private bool IsSeveritySufficient(string alertSeverity, string channelMinimumSeverity)
    {
        // Assuming severities are "Info", "Warning", "Critical" or similar ordinal values
        var severityOrder = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            { "Info", 0 },
            { "Information", 0 },
            { "Debug", -1 }, // Lower than Info
            { "Warning", 1 },
            { "Error", 2 },
            { "Critical", 3 },
            { "Fatal", 3 }
        };

        if (severityOrder.TryGetValue(alertSeverity, out int alertLevel) &&
            severityOrder.TryGetValue(channelMinimumSeverity, out int channelLevel))
        {
            return alertLevel >= channelLevel;
        }
        _logger.Warning($"Could not compare severities: Alert='{alertSeverity}', ChannelMin='{channelMinimumSeverity}'. Assuming sufficient.");
        return true; // Default to true if severities are unknown, to avoid missing alerts due to misconfiguration
    }

    private async Task DispatchToChannelAsync(
        IAlertingChannel channel,
        NotificationPayloadDto payload,
        AlertContextDto alertContext, // Pass original context for richer logging
        CancellationToken cancellationToken)
    {
        try
        {
            await channel.DispatchAlertAsync(payload, cancellationToken);
            _logger.Info($"Successfully dispatched alert for rule '{alertContext.TriggeredRuleName}' via {payload.TargetChannelType} channel.");
            await _auditLoggingAdapter.LogAuditEventAsync(
                eventType: "AlertDispatched",
                eventDetails: $"Rule: {alertContext.TriggeredRuleName}, Severity: {alertContext.AlertSeverity}, Channel: {payload.TargetChannelType}, Message: {alertContext.Message}",
                outcome: "Success",
                sourceComponent: nameof(AlertDispatchService));
        }
        catch (AlertingSystemException ex)
        {
            _logger.Error(ex, $"AlertingSystemException while dispatching alert for rule '{alertContext.TriggeredRuleName}' via {payload.TargetChannelType} channel.");
            await _auditLoggingAdapter.LogAuditEventAsync(
                eventType: "AlertDispatchFailed",
                eventDetails: $"Rule: {alertContext.TriggeredRuleName}, Severity: {alertContext.AlertSeverity}, Channel: {payload.TargetChannelType}, Error: {ex.Message}, Message: {alertContext.Message}",
                outcome: "Failure",
                sourceComponent: nameof(AlertDispatchService));
        }
        catch (Exception ex) // Catch any other unexpected errors
        {
            _logger.Error(ex, $"Unexpected error while dispatching alert for rule '{alertContext.TriggeredRuleName}' via {payload.TargetChannelType} channel.");
            await _auditLoggingAdapter.LogAuditEventAsync(
                eventType: "AlertDispatchFailed",
                eventDetails: $"Rule: {alertContext.TriggeredRuleName}, Severity: {alertContext.AlertSeverity}, Channel: {payload.TargetChannelType}, Unexpected Error: {ex.Message}, Message: {alertContext.Message}",
                outcome: "Failure",
                sourceComponent: nameof(AlertDispatchService));
        }
    }
}