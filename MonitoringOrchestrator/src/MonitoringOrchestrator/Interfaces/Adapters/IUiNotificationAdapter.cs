using TheSSS.DICOMViewer.Monitoring.Contracts;
using System.Threading.Tasks;

namespace TheSSS.DICOMViewer.Monitoring.Interfaces.Adapters;

/// <summary>
/// Adapter interface for sending notifications to the application's User Interface.
/// This abstracts interaction with UI notification systems, potentially via an event bus or a service exposed by the Presentation layer.
/// </summary>
public interface IUiNotificationAdapter
{
    /// <summary>
    /// Sends a notification to be displayed in the application UI.
    /// </summary>
    /// <param name="payload">The notification payload containing details to be displayed.</param>
    /// <returns>A task that represents the asynchronous notification sending operation.</returns>
    Task SendUiNotificationAsync(NotificationPayloadDto payload);
}