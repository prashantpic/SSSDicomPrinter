using System.Threading.Tasks;
using TheSSS.DICOMViewer.Monitoring.Contracts;

namespace TheSSS.DICOMViewer.Monitoring.Interfaces.Adapters;

/// <summary>
/// Adapter interface for sending notifications to the UI (likely via an event bus or Presentation layer service from REPO-WPF-UI or REPO-MASTER-UI-001).
/// </summary>
public interface IUiNotificationAdapter
{
    /// <summary>
    /// Sends a notification to be displayed in the application UI.
    /// This method is often fire-and-forget, but returns a Task for async consistency.
    /// </summary>
    /// <param name="payload">The notification payload, containing title, body, severity, etc.</param>
    /// <returns>A Task representing the asynchronous notification sending operation.</returns>
    /// <exception cref="TheSSS.DICOMViewer.Monitoring.Exceptions.AlertingSystemException">
    /// Thrown if sending the UI notification fails (e.g., UI service unavailable, invalid payload).
    /// </exception>
    Task SendUiNotificationAsync(NotificationPayloadDto payload);
}