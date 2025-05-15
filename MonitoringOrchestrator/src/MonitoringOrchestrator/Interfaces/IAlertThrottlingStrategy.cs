namespace TheSSS.DICOMViewer.Monitoring.Interfaces;

using System.Threading;
using System.Threading.Tasks;
using TheSSS.DICOMViewer.Monitoring.Contracts;

public interface IAlertThrottlingStrategy
{
    Task<bool> ShouldThrottleAsync(AlertContextDto alertContext, CancellationToken cancellationToken);
}