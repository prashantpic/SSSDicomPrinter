namespace TheSSS.DICOMViewer.Monitoring.Interfaces;

public interface IAlertingChannel
{
    string ChannelType { get; }
    Task DispatchAlertAsync(NotificationPayloadDto payload, CancellationToken cancellationToken);
}