namespace TheSSS.DICOMViewer.Monitoring.Interfaces.Adapters;

using TheSSS.DICOMViewer.Monitoring.Contracts;

/// <summary>
/// Adapter interface for sending notifications to the UI.
/// Implementation is expected to be provided by the UI layer (e.g., REPO-WPF-UI or REPO-MASTER-UI-001)
/// or an intermediary service.
/// </summary>
public interface IUiNotificationAdapter
{
    /// <summary>
    /// Sends a notification to be displayed in the application UI.
    /// </summary>
    /// <param name="payload">The notification payload, containing title, body, severity, etc.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task SendUiNotificationAsync(NotificationPayloadDto payload);
}