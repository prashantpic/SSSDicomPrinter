using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TheSSS.DICOMViewer.Monitoring.Configuration;
using TheSSS.DICOMViewer.Monitoring.Contracts;
using TheSSS.DICOMViewer.Monitoring.Exceptions;
using TheSSS.DICOMViewer.Monitoring.Interfaces;
using TheSSS.DICOMViewer.Monitoring.Interfaces.Adapters; // Assuming ILoggerAdapter and IEmailServiceAdapter
using System.Collections.Generic; // Required for IEnumerable

namespace TheSSS.DICOMViewer.Monitoring.Alerting.Channels;

public class EmailAlertingChannel : IAlertingChannel
{
    private readonly IEmailServiceAdapter _emailServiceAdapter;
    private readonly ILoggerAdapter<EmailAlertingChannel> _logger;
    // private readonly AlertingOptions _alertingOptions; // Could inject options if needed for channel-specific settings

    public EmailAlertingChannel(IEmailServiceAdapter emailServiceAdapter, ILoggerAdapter<EmailAlertingChannel> logger /*, IOptions<AlertingOptions> alertingOptions*/)
    {
        _emailServiceAdapter = emailServiceAdapter;
        _logger = logger;
        // _alertingOptions = alertingOptions.Value;
    }

    /// <inheritdoc/>
    public async Task DispatchAlertAsync(NotificationPayloadDto payload, CancellationToken cancellationToken)
    {
        _logger.Debug($"Attempting to dispatch email alert: '{payload.Title}'");

        if (payload.RecipientDetails is not IEnumerable<string> recipients || !recipients.Any())
        {
            _logger.Warning($"Email alert dispatch skipped: No recipient email addresses provided in payload.");
            throw new AlertingSystemException(payload.TargetChannelType, "No recipient email addresses provided for email channel.");
        }

         // Optional: Check if minimum severity matches this channel's configuration
         // This would require injecting IOptions<AlertingOptions> and finding this channel's setting.
         // For simplicity, assume filtering is done by AlertDispatchService.

        try
        {
            // Assuming subject and body are already formatted in the payload
            await _emailServiceAdapter.SendEmailAsync(recipients, payload.Title, payload.Body, cancellationToken);
            _logger.Info($"Successfully dispatched email alert to {string.Join(", ", recipients)}.");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"Failed to send email alert to {string.Join(", ", recipients)}.");
            // Wrap specific email sending errors in AlertingSystemException
            throw new AlertingSystemException(payload.TargetChannelType, payload.Title, "Email dispatch failed.", ex);
        }
    }
}