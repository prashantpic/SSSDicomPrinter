using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TheSSS.DICOMViewer.Monitoring.Configuration;
using TheSSS.DICOMViewer.Monitoring.Contracts;
using TheSSS.DICOMViewer.Monitoring.Exceptions;
using TheSSS.DICOMViewer.Monitoring.Interfaces;
using TheSSS.DICOMViewer.Monitoring.Interfaces.Adapters;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace TheSSS.DICOMViewer.Monitoring.Alerting.Channels
{
    /// <summary>
    /// Implementation of <see cref="IAlertingChannel"/> for sending alerts via email.
    /// Dispatches alerts as email notifications to configured administrator addresses.
    /// </summary>
    public class EmailAlertingChannel : IAlertingChannel
    {
        private const string ChannelTypeValue = "Email";
        private readonly IEmailServiceAdapter _emailServiceAdapter;
        private readonly IOptions<AlertingOptions> _alertingOptions;
        private readonly ILogger<EmailAlertingChannel> _logger;
        private static readonly Dictionary<string, int> SeverityOrder = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            { "Information", 1 },
            { "Warning", 2 },
            { "Error", 3 },
            { "Critical", 4 }
        };


        /// <summary>
        /// Initializes a new instance of the <see cref="EmailAlertingChannel"/> class.
        /// </summary>
        /// <param name="emailServiceAdapter">The adapter for sending emails.</param>
        /// <param name="alertingOptions">The alerting configuration options.</param>
        /// <param name="logger">The logger.</param>
        public EmailAlertingChannel(
            IEmailServiceAdapter emailServiceAdapter,
            IOptions<AlertingOptions> alertingOptions,
            ILogger<EmailAlertingChannel> logger)
        {
            _emailServiceAdapter = emailServiceAdapter ?? throw new ArgumentNullException(nameof(emailServiceAdapter));
            _alertingOptions = alertingOptions ?? throw new ArgumentNullException(nameof(alertingOptions));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc/>
        public async Task DispatchAlertAsync(NotificationPayloadDto payload, CancellationToken cancellationToken)
        {
            var emailChannelSetting = _alertingOptions.Value.Channels
                .FirstOrDefault(c => ChannelTypeValue.Equals(c.ChannelType, StringComparison.OrdinalIgnoreCase) && c.IsEnabled);

            if (emailChannelSetting == null)
            {
                _logger.LogDebug("Email alerting channel is not configured or not enabled.");
                return;
            }

            if (!IsSeveritySufficient(payload.Severity, emailChannelSetting.MinimumSeverity))
            {
                _logger.LogInformation("Alert severity '{PayloadSeverity}' for rule '{RuleName}' on component '{SourceComponent}' does not meet minimum '{MinimumSeverity}' for Email channel. Skipping.",
                    payload.Severity, payload.Title, payload.SourceComponent, emailChannelSetting.MinimumSeverity);
                return;
            }
            
            if (emailChannelSetting.RecipientEmailAddresses == null || !emailChannelSetting.RecipientEmailAddresses.Any())
            {
                _logger.LogWarning("Email alerting channel is enabled but no recipient email addresses are configured.");
                return;
            }

            _logger.LogInformation("Dispatching alert via Email: {Title}", payload.Title);

            foreach (var recipient in emailChannelSetting.RecipientEmailAddresses)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    _logger.LogWarning("Email alert dispatch cancelled for recipient {Recipient}.", recipient);
                    return;
                }

                try
                {
                    await _emailServiceAdapter.SendEmailAsync(recipient, payload.Title, payload.Body);
                    _logger.LogInformation("Successfully sent email alert to {Recipient} for: {Title}", recipient, payload.Title);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send email alert to {Recipient} for: {Title}", recipient, payload.Title);
                    // Potentially throw a single AlertingSystemException after trying all recipients,
                    // or just log and continue for resilience. For now, log and continue.
                    // If one email fails, others might succeed.
                }
            }
        }
        
        private bool IsSeveritySufficient(string payloadSeverity, string? minimumSeverity)
        {
            if (string.IsNullOrEmpty(minimumSeverity))
            {
                return true; // No minimum severity defined, always sufficient
            }

            if (SeverityOrder.TryGetValue(payloadSeverity, out int payloadSeverityValue) &&
                SeverityOrder.TryGetValue(minimumSeverity, out int minimumSeverityValue))
            {
                return payloadSeverityValue >= minimumSeverityValue;
            }

            _logger.LogWarning("Could not compare severities: Payload='{PayloadSeverity}', Minimum='{MinimumSeverity}'. Assuming insufficient.", payloadSeverity, minimumSeverity);
            return false; // If severities are unknown, treat as insufficient to be safe
        }
    }
}