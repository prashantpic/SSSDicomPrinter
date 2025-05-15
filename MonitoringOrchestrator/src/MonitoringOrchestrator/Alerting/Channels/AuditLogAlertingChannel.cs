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
using System.Text.Json;
using System.Collections.Generic;

namespace TheSSS.DICOMViewer.Monitoring.Alerting.Channels
{
    /// <summary>
    /// Implementation of <see cref="IAlertingChannel"/> for logging alerts to the audit trail.
    /// Logs the occurrence and details of dispatched alerts to the audit log.
    /// </summary>
    public class AuditLogAlertingChannel : IAlertingChannel
    {
        private const string ChannelTypeValue = "AuditLog";
        private readonly IAuditLoggingAdapter _auditLoggingAdapter;
        private readonly IOptions<AlertingOptions> _alertingOptions;
        private readonly ILogger<AuditLogAlertingChannel> _logger;
        private static readonly Dictionary<string, int> SeverityOrder = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            { "Information", 1 },
            { "Warning", 2 },
            { "Error", 3 },
            { "Critical", 4 }
        };
        
        /// <summary>
        /// Initializes a new instance of the <see cref="AuditLogAlertingChannel"/> class.
        /// </summary>
        /// <param name="auditLoggingAdapter">The adapter for logging audit events.</param>
        /// <param name="alertingOptions">The alerting configuration options.</param>
        /// <param name="logger">The logger.</param>
        public AuditLogAlertingChannel(
            IAuditLoggingAdapter auditLoggingAdapter,
            IOptions<AlertingOptions> alertingOptions,
            ILogger<AuditLogAlertingChannel> logger)
        {
            _auditLoggingAdapter = auditLoggingAdapter ?? throw new ArgumentNullException(nameof(auditLoggingAdapter));
            _alertingOptions = alertingOptions ?? throw new ArgumentNullException(nameof(alertingOptions));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc/>
        public async Task DispatchAlertAsync(NotificationPayloadDto payload, CancellationToken cancellationToken)
        {
            var auditLogChannelSetting = _alertingOptions.Value.Channels
                .FirstOrDefault(c => ChannelTypeValue.Equals(c.ChannelType, StringComparison.OrdinalIgnoreCase) && c.IsEnabled);

            if (auditLogChannelSetting == null)
            {
                _logger.LogDebug("AuditLog alerting channel is not configured or not enabled.");
                return;
            }
            
            if (!IsSeveritySufficient(payload.Severity, auditLogChannelSetting.MinimumSeverity))
            {
                _logger.LogInformation("Alert severity '{PayloadSeverity}' for rule '{RuleName}' on component '{SourceComponent}' does not meet minimum '{MinimumSeverity}' for AuditLog channel. Skipping.",
                    payload.Severity, payload.Title, payload.SourceComponent, auditLogChannelSetting.MinimumSeverity);
                return;
            }

            _logger.LogInformation("Dispatching alert via AuditLog: {Title}", payload.Title);

            try
            {
                // Serialize the payload or relevant parts for detailed audit logging
                var eventDetails = JsonSerializer.Serialize(new 
                {
                    payload.Title,
                    payload.Severity,
                    payload.Timestamp,
                    payload.SourceComponent,
                    MessageBody = payload.Body // Or a summary if body is too long
                });

                // Log an audit event for the dispatched system alert
                // Parameters for LogAuditEventAsync: eventType, eventDetails, outcome, sourceComponent
                await _auditLoggingAdapter.LogAuditEventAsync(
                    eventType: "SystemAlertDispatched",
                    eventDetails: eventDetails,
                    outcome: "Success", // Outcome of logging the alert itself
                    sourceComponent: $"MonitoringOrchestrator.{payload.SourceComponent}" 
                );

                _logger.LogInformation("Successfully logged alert to audit trail: {Title}", payload.Title);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to log alert to audit trail: {Title}", payload.Title);
                throw new AlertingSystemException($"Failed to log alert to audit trail: {ex.Message}", ex, ChannelTypeValue);
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
            _logger.LogWarning("Could not compare severities for AuditLog Channel: Payload='{PayloadSeverity}', Minimum='{MinimumSeverity}'. Assuming insufficient.", payloadSeverity, minimumSeverity);
            return false; 
        }
    }
}