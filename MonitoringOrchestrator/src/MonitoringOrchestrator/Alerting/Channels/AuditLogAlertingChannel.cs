using TheSSS.DICOMViewer.Monitoring.Interfaces;
using TheSSS.DICOMViewer.Monitoring.Interfaces.Adapters;
using TheSSS.DICOMViewer.Monitoring.Contracts;
using TheSSS.DICOMViewer.Monitoring.Configuration;
using TheSSS.DICOMViewer.Monitoring.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace TheSSS.DICOMViewer.Monitoring.Alerting.Channels
{
    public class AuditLogAlertingChannel : IAlertingChannel
    {
        private readonly IAuditLoggingAdapter _auditLoggingAdapter;
        private readonly AlertingOptions _alertingOptions; // To check if AuditLog channel is enabled in config
        private readonly ILogger<AuditLogAlertingChannel> _logger;
        private static readonly JsonSerializerOptions _jsonSerializerOptions = new() { WriteIndented = false };


        public string ChannelType => "AuditLog";

        public AuditLogAlertingChannel(
            IAuditLoggingAdapter auditLoggingAdapter,
            IOptions<AlertingOptions> alertingOptions,
            ILogger<AuditLogAlertingChannel> logger)
        {
            _auditLoggingAdapter = auditLoggingAdapter ?? throw new ArgumentNullException(nameof(auditLoggingAdapter));
            _alertingOptions = alertingOptions?.Value ?? throw new ArgumentNullException(nameof(alertingOptions));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task DispatchAlertAsync(NotificationPayloadDto payload, CancellationToken cancellationToken)
        {
            if (payload == null) throw new ArgumentNullException(nameof(payload));

            _logger.LogInformation("Attempting to log alert to {ChannelType} for rule: {RuleName}, Severity: {Severity}.", ChannelType, payload.TriggeredRuleName, payload.Severity);
            
            var auditLogChannelSetting = _alertingOptions.Channels
                .FirstOrDefault(c => c.ChannelType.Equals(ChannelType, StringComparison.OrdinalIgnoreCase) && c.IsEnabled);

            if (auditLogChannelSetting == null)
            {
                _logger.LogWarning("{ChannelType} alerting channel is not configured or not enabled. Skipping dispatch for rule: {RuleName}.", ChannelType, payload.TriggeredRuleName);
                return; // Not an error, just not configured
            }

            try
            {
                var eventType = "SystemAlertDispatched"; // Could be more specific based on payload.Severity
                var eventDetails = JsonSerializer.Serialize(payload, _jsonSerializerOptions);
                var outcome = "Success"; // Logging the alert itself is considered a success here

                await _auditLoggingAdapter.LogAuditEventAsync(
                    eventType,
                    eventDetails,
                    outcome,
                    payload.SourceComponent // SourceComponent from the payload
                );

                _logger.LogInformation("Successfully logged alert to {ChannelType} for rule: {RuleName}.", ChannelType, payload.TriggeredRuleName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to log alert to {ChannelType} for rule: {RuleName}.", ChannelType, payload.TriggeredRuleName);
                // Avoid throwing AlertingSystemException if audit logging fails, as it might prevent other channels
                // Or, if audit logging is critical, then do throw. For now, log and continue.
                // Let's reconsider: if this channel fails, it IS an AlertingSystemException for THIS channel.
                throw new AlertingSystemException(ChannelType, $"Failed to log alert to audit trail for rule: {payload.TriggeredRuleName}.", payload, ex);
            }
        }
    }
}