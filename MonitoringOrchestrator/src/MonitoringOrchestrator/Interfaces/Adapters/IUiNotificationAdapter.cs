namespace TheSSS.DICOMViewer.Monitoring.Interfaces.Adapters;

using System.Threading.Tasks;
using TheSSS.DICOMViewer.Monitoring.Contracts;

public interface IUiNotificationAdapter
{
    Task SendUiNotificationAsync(NotificationPayloadDto payload);
}