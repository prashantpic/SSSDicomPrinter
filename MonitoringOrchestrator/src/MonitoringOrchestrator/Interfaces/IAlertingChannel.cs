using System.Threading;
using System.Threading.Tasks;
using TheSSS.DICOMViewer.Monitoring.Contracts;

namespace TheSSS.DICOMViewer.Monitoring.Interfaces;

/// <summary>
/// Interface for components that can dispatch alerts through a specific channel.
/// </summary>
public interface IAlertingChannel
{
    /// <summary>
    /// Dispatches an alert using the specific channel's mechanism asynchronously.
    /// Implementations should handle their own channel-specific logic (e.g., formatting, sending).
    /// </summary>
    /// <param name="payload">The notification payload containing alert details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Task representing the asynchronous dispatch operation.</returns>
    /// <exception cref="TheSSS.DICOMViewer.Monitoring.Exceptions.AlertingSystemException">
    /// Thrown if the dispatch fails due to an issue with the alerting channel or downstream system.
    /// </exception>
    /// <exception cref="System.OperationCanceledException">
    /// Thrown if the operation is cancelled via the cancellation token.
    /// </exception>
    Task DispatchAlertAsync(NotificationPayloadDto payload, CancellationToken cancellationToken);
}