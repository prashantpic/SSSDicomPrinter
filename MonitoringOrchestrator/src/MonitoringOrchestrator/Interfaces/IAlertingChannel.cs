namespace TheSSS.DICOMViewer.Monitoring.Interfaces;

using System.Threading;
using System.Threading.Tasks;
using TheSSS.DICOMViewer.Monitoring.Contracts;

public interface IAlertingChannel
{
    string ChannelType { get; }
    Task DispatchAlertAsync(NotificationPayloadDto payload, CancellationToken cancellationToken);
}