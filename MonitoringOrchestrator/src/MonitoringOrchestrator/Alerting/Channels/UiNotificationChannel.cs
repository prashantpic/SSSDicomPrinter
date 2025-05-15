using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TheSSS.DICOMViewer.Monitoring.Contracts;
using TheSSS.DICOMViewer.Monitoring.Exceptions;
using TheSSS.DICOMViewer.Monitoring.Interfaces;
using TheSSS.DICOMViewer.Monitoring.Interfaces.Adapters; // Assuming ILoggerAdapter and IUiNotificationAdapter

namespace TheSSS.DICOMViewer.Monitoring.Alerting.Channels;

public class UiNotificationChannel : IAlertingChannel
{
    private readonly IUiNotificationAdapter _uiNotificationAdapter;
    private readonly ILoggerAdapter<UiNotificationChannel> _logger;

    public UiNotificationChannel(IUiNotificationAdapter uiNotificationAdapter, ILoggerAdapter<UiNotificationChannel> logger)
    {
        _uiNotificationAdapter = uiNotificationAdapter;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task DispatchAlertAsync(NotificationPayloadDto payload, CancellationToken cancellationToken)
    {
         _logger.Debug($"Attempting to dispatch UI notification: '{payload.Title}'");

        try
        {
            // UI notifications typically don't need a cancellation token passed down to the adapter
            // as the adapter might interact with an event bus or fire-and-forget mechanism.
            // We still accept it in the interface method signature for consistency.
            await _uiNotificationAdapter.SendUiNotificationAsync(payload);
            _logger.Info("Successfully dispatched UI notification.");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to send UI notification.");
            // Wrap specific UI notification errors in AlertingSystemException
            throw new AlertingSystemException(payload.TargetChannelType, payload.Title, "UI notification dispatch failed.", ex);
        }
    }
}