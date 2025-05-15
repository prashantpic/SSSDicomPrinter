namespace TheSSS.DICOMViewer.Monitoring.Interfaces.Adapters;

public interface IUiNotificationAdapter
{
    Task SendUiNotificationAsync(NotificationPayloadDto payload);
}