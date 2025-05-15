using TheSSS.DICOMViewer.Monitoring.Contracts;
using TheSSS.DICOMViewer.Monitoring.Interfaces;
using TheSSS.DICOMViewer.Monitoring.Interfaces.Adapters;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheSSS.DICOMViewer.Monitoring.UseCaseHandlers;

public class AlertDispatchService
{
    private readonly IEnumerable<IAlertingChannel> _channels;
    private readonly IAlertThrottlingStrategy _throttling;
    private readonly IAlertDeduplicationStrategy _deduplication;
    private readonly IAuditLoggingAdapter _audit;
    private readonly ILogger<AlertDispatchService> _logger;

    public AlertDispatchService(
        IEnumerable<IAlertingChannel> channels,
        IAlertThrottlingStrategy throttling,
        IAlertDeduplicationStrategy deduplication,
        IAuditLoggingAdapter audit,
        ILogger<AlertDispatchService> logger)
    {
        _channels = channels;
        _throttling = throttling;
        _deduplication = deduplication;
        _audit = audit;
        _logger = logger;
    }

    public async Task DispatchAlertAsync(AlertContextDto context)
    {
        if (await _throttling.ShouldThrottleAsync(context) || await _deduplication.IsDuplicateAsync(context))
        {
            _logger.LogWarning($"Alert {context.TriggeredRuleName} suppressed");
            return;
        }

        var payload = new NotificationPayloadDto
        {
            Title = $"{context.Severity} Alert: {context.TriggeredRuleName}",
            Body = context.Message,
            Severity = context.Severity,
            Timestamp = DateTime.UtcNow
        };

        foreach (var channel in _channels.Where(c => c.ChannelType != "AuditLog"))
        {
            try
            {
                await channel.DispatchAlertAsync(payload);
                await _audit.LogAuditEventAsync("AlertDispatched", 
                    $"Alert {context.TriggeredRuleName} sent via {channel.ChannelType}", 
                    "Success", 
                    "MonitoringOrchestrator");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to dispatch via {channel.ChannelType}");
                await _audit.LogAuditEventAsync("AlertDispatchFailed",
                    $"Failed to send {context.TriggeredRuleName} via {channel.ChannelType}: {ex.Message}",
                    "Failure",
                    "MonitoringOrchestrator");
            }
        }
    }
}