using TheSSS.DICOMViewer.Monitoring.Interfaces;
using TheSSS.DICOMViewer.Monitoring.Interfaces.Adapters;
using TheSSS.DICOMViewer.Monitoring.Contracts;
using TheSSS.DICOMViewer.Monitoring.Configuration;
using TheSSS.DICOMViewer.Monitoring.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TheSSS.DICOMViewer.Monitoring.Alerting.Channels
{
    public class UiNotificationChannel : IAlertingChannel
    {
        private readonly IUiNotificationAdapter _uiNotificationAdapter;
        private readonly AlertingOptions _alertingOptions; // To check if UI channel is enabled in config
        private readonly ILogger<UiNotificationChannel> _logger;

        public string ChannelType => "UI";

        public UiNotificationChannel(
            IUiNotificationAdapter uiNotificationAdapter,
            IOptions<AlertingOptions> alertingOptions,
            ILogger<UiNotificationChannel> logger)
        {
            _uiNotificationAdapter = uiNotificationAdapter ?? throw new ArgumentNullException(nameof(uiNotificationAdapter));
            _alertingOptions = alertingOptions?.Value ?? throw new ArgumentNullException(nameof(alertingOptions));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task DispatchAlertAsync(NotificationPayloadDto payload, CancellationToken cancellationToken)
        {
            if (payload == null) throw new ArgumentNullException(nameof(payload));

            _logger.LogInformation("Attempting to dispatch alert via {ChannelType} for rule: {RuleName}, Severity: {Severity}.", ChannelType, payload.TriggeredRuleName, payload.Severity);

            var uiChannelSetting = _alertingOptions.Channels
                .FirstOrDefault(c => c.ChannelType.Equals(ChannelType, StringComparison.OrdinalIgnoreCase) && c.IsEnabled);

            if (uiChannelSetting == null)
            {
                _logger.LogWarning("{ChannelType} alerting channel is not configured or not enabled. Skipping dispatch for rule: {RuleName}.", ChannelType, payload.TriggeredRuleName);
                return; // Not an error, just not configured
            }

            try
            {
                await _uiNotificationAdapter.SendUiNotificationAsync(payload);
                _logger.LogInformation("Successfully dispatched alert via {ChannelType} for rule: {RuleName}.", ChannelType, payload.TriggeredRuleName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to dispatch alert via {ChannelType} for rule: {RuleName}.", ChannelType, payload.TriggeredRuleName);
                throw new AlertingSystemException(ChannelType, $"Failed to send UI notification for alert: {payload.TriggeredRuleName}.", payload, ex);
            }
        }
    }
}