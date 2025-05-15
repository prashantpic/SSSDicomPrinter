namespace TheSSS.DICOMViewer.Monitoring.Interfaces;

public interface IHealthDataSource
{
    string Name { get; }
    Task<object> GetHealthDataAsync(CancellationToken cancellationToken);
}