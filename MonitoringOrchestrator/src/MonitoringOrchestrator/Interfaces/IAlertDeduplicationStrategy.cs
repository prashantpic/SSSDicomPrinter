namespace TheSSS.DICOMViewer.Monitoring.Interfaces;

public interface IAlertDeduplicationStrategy
{
    Task<bool> IsDuplicateAsync(AlertContextDto alertContext, CancellationToken cancellationToken);
    void RegisterProcessedAlert(AlertContextDto alertContext);
}