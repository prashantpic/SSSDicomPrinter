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
    public class EmailAlertingChannel : IAlertingChannel
    {
        private readonly IEmailServiceAdapter _emailServiceAdapter;
        private readonly AlertingOptions _alertingOptions;
        private readonly ILogger<EmailAlertingChannel> _logger;

        public string ChannelType => "Email";

        public EmailAlertingChannel(
            IEmailServiceAdapter emailServiceAdapter,
            IOptions<AlertingOptions> alertingOptions,
            ILogger<EmailAlertingChannel> logger)
        {
            _emailServiceAdapter = emailServiceAdapter ?? throw new ArgumentNullException(nameof(emailServiceAdapter));
            _alertingOptions = alertingOptions?.Value ?? throw new ArgumentNullException(nameof(alertingOptions));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task DispatchAlertAsync(NotificationPayloadDto payload, CancellationToken cancellationToken)
        {
            if (payload == null) throw new ArgumentNullException(nameof(payload));

            _logger.LogInformation("Attempting to dispatch alert via {ChannelType} for rule: {RuleName}, Severity: {Severity}.", ChannelType, payload.TriggeredRuleName, payload.Severity);

            var emailChannelSetting = _alertingOptions.Channels
                .FirstOrDefault(c => c.ChannelType.Equals(ChannelType, StringComparison.OrdinalIgnoreCase) && c.IsEnabled);

            if (emailChannelSetting == null)
            {
                _logger.LogWarning("{ChannelType} alerting channel is not configured or not enabled. Skipping dispatch for rule: {RuleName}.", ChannelType, payload.TriggeredRuleName);
                return; // Not an error, just not configured for this channel
            }

            if (emailChannelSetting.RecipientDetails == null || !emailChannelSetting.RecipientDetails.Any())
            {
                _logger.LogError("{ChannelType} alerting channel is enabled but has no recipient email addresses configured for rule: {RuleName}.", ChannelType, payload.TriggeredRuleName);
                throw new AlertingSystemException(ChannelType, "No recipient email addresses configured for the Email channel.", payload, new InvalidOperationException("RecipientDetails are missing."));
            }

            try
            {
                await _emailServiceAdapter.SendEmailAsync(emailChannelSetting.RecipientDetails, payload.Title, payload.Body);
                _logger.LogInformation("Successfully dispatched alert via {ChannelType} to {RecipientCount} recipients for rule: {RuleName}.", ChannelType, emailChannelSetting.RecipientDetails.Count, payload.TriggeredRuleName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to dispatch alert via {ChannelType} for rule: {RuleName}.", ChannelType, payload.TriggeredRuleName);
                throw new AlertingSystemException(ChannelType, $"Failed to send email for alert: {payload.TriggeredRuleName}.", payload, ex);
            }
        }
    }
}