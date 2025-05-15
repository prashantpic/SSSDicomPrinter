using TheSSS.DICOMViewer.Monitoring.Contracts;
using TheSSS.DICOMViewer.Monitoring.Interfaces;
using TheSSS.DICOMViewer.Monitoring.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TheSSS.DICOMViewer.Monitoring.UseCaseHandlers;

public class AlertDispatchService
{
    private readonly IEnumerable<IAlertingChannel> _alertingChannels;
    private readonly IAlertThrottlingStrategy _throttlingStrategy;
    private readonly IAlertDeduplicationStrategy _deduplicationStrategy;
    private readonly AlertingOptions _alertingOptions;
    private readonly ILogger<AlertDispatchService> _logger;

    public AlertDispatchService(
        IEnumerable<IAlertingChannel> alertingChannels,
        IAlertThrottlingStrategy throttlingStrategy,
        IAlertDeduplicationStrategy deduplicationStrategy,
        IOptions<AlertingOptions> alertingOptions,
        ILogger<AlertDispatchService> logger)
    {
        _alertingChannels = alertingChannels;
        _throttlingStrategy = throttlingStrategy;
        _deduplicationStrategy = deduplicationStrategy;
        _alertingOptions = alertingOptions.Value;
        _logger = logger;
    }

    public async Task DispatchAlertAsync(AlertContextDto alertContext, CancellationToken cancellationToken)
    {
        if (await _deduplicationStrategy.IsDuplicateAsync(alertContext, cancellationToken))
        {
            _logger.LogWarning("Duplicate alert detected: {AlertId}", alertContext.AlertInstanceId);
            return;
        }

        if (await _throttlingStrategy.ShouldThrottleAsync(alertContext, cancellationToken))
        {
            _logger.LogWarning("Alert throttled: {AlertId}", alertContext.AlertInstanceId);
            return;
        }

        var payload = new NotificationPayloadDto
        {
            Title = $"Alert: {alertContext.TriggeredRuleName}",
            Body = alertContext.Message,
            Severity = alertContext.Severity,
            Timestamp = DateTime.UtcNow,
            TargetChannelType = "All",
            SourceComponent = alertContext.SourceComponent
        };

        foreach (var channel in _alertingChannels.Where(c => IsChannelEnabled(c.ChannelType)))
        {
            try
            {
                await channel.DispatchAlertAsync(payload, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to dispatch alert via {ChannelType}", channel.ChannelType);
            }
        }
    }

    private bool IsChannelEnabled(string channelType)
    {
        return _alertingOptions.Channels?
            .Any(c => c.ChannelType == channelType && c.IsEnabled) ?? false;
    }
}