namespace TheSSS.DICOMViewer.Monitoring.Interfaces;

using System.Threading;
using System.Threading.Tasks;
using TheSSS.DICOMViewer.Monitoring.Contracts;

public interface IAlertDeduplicationStrategy
{
    Task<bool> IsDuplicateAsync(AlertContextDto alertContext, CancellationToken cancellationToken);
    void RegisterProcessedAlert(AlertContextDto alertContext);
}