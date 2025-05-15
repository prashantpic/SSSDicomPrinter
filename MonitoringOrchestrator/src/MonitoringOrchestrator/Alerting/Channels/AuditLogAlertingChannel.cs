using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TheSSS.DICOMViewer.Monitoring.Contracts;
using TheSSS.DICOMViewer.Monitoring.Exceptions;
using TheSSS.DICOMViewer.Monitoring.Interfaces;
using TheSSS.DICOMViewer.Monitoring.Interfaces.Adapters; // Assuming ILoggerAdapter and IAuditLoggingAdapter

namespace TheSSS.DICOMViewer.Monitoring.Alerting.Channels;

public class AuditLogAlertingChannel : IAlertingChannel
{
    private readonly IAuditLoggingAdapter _auditLoggingAdapter;
    private readonly ILoggerAdapter<AuditLogAlertingChannel> _logger;

    public AuditLogAlertingChannel(IAuditLoggingAdapter auditLoggingAdapter, ILoggerAdapter<AuditLogAlertingChannel> logger)
    {
        _auditLoggingAdapter = auditLoggingAdapter;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task DispatchAlertAsync(NotificationPayloadDto payload, CancellationToken cancellationToken)
    {
        _logger.Debug($"Attempting to log alert to audit trail: '{payload.Title}'");

        try
        {
            // Construct audit log details from the notification payload
            var eventDetails = $"System Alert Triggered: {payload.Title}\n" +
                               $"Severity: {payload.Severity}\n" +
                               $"Message: {payload.Body}\n" +
                               $"Timestamp: {payload.Timestamp:yyyy-MM-dd HH:mm:ss}";
            // Note: The source component might be better derived earlier in the process,
            // but for this channel, logging the fact it was dispatched is key.
            var sourceComponent = "Monitoring.Alerting.AuditLogChannel";
            var outcome = "AlertLogged"; // Specific outcome for audit logging

            await _auditLoggingAdapter.LogAuditEventAsync(
                eventType: "SystemAlert", // or "AlertDispatched"
                eventDetails: eventDetails,
                outcome: outcome,
                sourceComponent: sourceComponent
                // userId, studyInstanceUid might not be available here
            );
            _logger.Info("Successfully logged alert to audit trail.");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to log alert to audit trail.");
            // Wrap specific audit logging errors in AlertingSystemException
            throw new AlertingSystemException(payload.TargetChannelType, payload.Title, "Audit log dispatch failed.", ex);
        }
    }
}