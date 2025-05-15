using TheSSS.DICOMViewer.Monitoring.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace TheSSS.DICOMViewer.Monitoring.Interfaces;

/// <summary>
/// Defines a contract for components that can dispatch alerts through a specific channel (e.g., email, UI notification).
/// </summary>
public interface IAlertingChannel
{
    /// <summary>
    /// Asynchronously dispatches an alert using the specific channel's mechanism.
    /// </summary>
    /// <param name="payload">The notification payload containing the alert details formatted for dispatch.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous dispatch operation.</returns>
    Task DispatchAlertAsync(NotificationPayloadDto payload, CancellationToken cancellationToken);
}