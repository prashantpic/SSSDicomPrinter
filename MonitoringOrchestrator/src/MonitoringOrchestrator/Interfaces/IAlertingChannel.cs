namespace TheSSS.DICOMViewer.Monitoring.Interfaces;

using TheSSS.DICOMViewer.Monitoring.Contracts;

/// <summary>
/// Interface for components that can dispatch alerts through a specific channel.
/// </summary>
public interface IAlertingChannel
{
    /// <summary>
    /// Gets the type identifier for this alerting channel (e.g., "Email", "UI", "AuditLog").
    /// This should match the ChannelType configured in AlertChannelSetting.
    /// </summary>
    string ChannelType { get; }

    /// <summary>
    /// Asynchronously dispatches an alert using the specific channel's mechanism.
    /// </summary>
    /// <param name="payload">The payload containing the notification details.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task DispatchAlertAsync(NotificationPayloadDto payload, CancellationToken cancellationToken);
}