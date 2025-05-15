namespace TheSSS.DICOMViewer.Monitoring.Interfaces;

public interface IAlertThrottlingStrategy
{
    Task<bool> ShouldThrottleAsync(AlertContextDto alertContext, CancellationToken cancellationToken);
}