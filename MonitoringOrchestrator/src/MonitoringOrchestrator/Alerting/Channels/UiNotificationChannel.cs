using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TheSSS.DICOMViewer.Monitoring.Configuration;
using TheSSS.DICOMViewer.Monitoring.Contracts;
using TheSSS.DICOMViewer.Monitoring.Exceptions;
using TheSSS.DICOMViewer.Monitoring.Interfaces;
using TheSSS.DICOMViewer.Monitoring.Interfaces.Adapters;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace TheSSS.DICOMViewer.Monitoring.Alerting.Channels
{
    /// <summary>
    /// Implementation of <see cref="IAlertingChannel"/> for sending alerts to the UI.
    /// Dispatches alerts for display within the application UI to administrative roles.
    /// </summary>
    public class UiNotificationChannel : IAlertingChannel
    {
        private const string ChannelTypeValue = "UI";
        private readonly IUiNotificationAdapter _uiNotificationAdapter;
        private readonly IOptions<AlertingOptions> _alertingOptions;
        private readonly ILogger<UiNotificationChannel> _logger;
        private static readonly Dictionary<string, int> SeverityOrder = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            { "Information", 1 },
            { "Warning", 2 },
            { "Error", 3 },
            { "Critical", 4 }
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="UiNotificationChannel"/> class.
        /// </summary>
        /// <param name="uiNotificationAdapter">The adapter for sending UI notifications.</param>
        /// <param name="alertingOptions">The alerting configuration options.</param>
        /// <param name="logger">The logger.</param>
        public UiNotificationChannel(
            IUiNotificationAdapter uiNotificationAdapter,
            IOptions<AlertingOptions> alertingOptions,
            ILogger<UiNotificationChannel> logger)
        {
            _uiNotificationAdapter = uiNotificationAdapter ?? throw new ArgumentNullException(nameof(uiNotificationAdapter));
            _alertingOptions = alertingOptions ?? throw new ArgumentNullException(nameof(alertingOptions));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc/>
        public async Task DispatchAlertAsync(NotificationPayloadDto payload, CancellationToken cancellationToken)
        {
            var uiChannelSetting = _alertingOptions.Value.Channels
                .FirstOrDefault(c => ChannelTypeValue.Equals(c.ChannelType, StringComparison.OrdinalIgnoreCase) && c.IsEnabled);

            if (uiChannelSetting == null)
            {
                _logger.LogDebug("UI notification channel is not configured or not enabled.");
                return;
            }
            
            if (!IsSeveritySufficient(payload.Severity, uiChannelSetting.MinimumSeverity))
            {
                 _logger.LogInformation("Alert severity '{PayloadSeverity}' for rule '{RuleName}' on component '{SourceComponent}' does not meet minimum '{MinimumSeverity}' for UI channel. Skipping.",
                    payload.Severity, payload.Title, payload.SourceComponent, uiChannelSetting.MinimumSeverity);
                return;
            }

            _logger.LogInformation("Dispatching alert via UI Notification: {Title}", payload.Title);

            try
            {
                await _uiNotificationAdapter.SendUiNotificationAsync(payload);
                _logger.LogInformation("Successfully sent UI notification for: {Title}", payload.Title);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send UI notification for: {Title}", payload.Title);
                throw new AlertingSystemException($"Failed to send UI notification: {ex.Message}", ex, ChannelTypeValue);
            }
        }
        
        private bool IsSeveritySufficient(string payloadSeverity, string? minimumSeverity)
        {
            if (string.IsNullOrEmpty(minimumSeverity))
            {
                return true; 
            }

            if (SeverityOrder.TryGetValue(payloadSeverity, out int payloadSeverityValue) &&
                SeverityOrder.TryGetValue(minimumSeverity, out int minimumSeverityValue))
            {
                return payloadSeverityValue >= minimumSeverityValue;
            }
            _logger.LogWarning("Could not compare severities for UI Channel: Payload='{PayloadSeverity}', Minimum='{MinimumSeverity}'. Assuming insufficient.", payloadSeverity, minimumSeverity);
            return false; 
        }
    }
}