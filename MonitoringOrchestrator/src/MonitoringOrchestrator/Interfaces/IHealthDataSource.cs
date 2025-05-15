namespace TheSSS.DICOMViewer.Monitoring.Interfaces;

using System.Threading;
using System.Threading.Tasks;

public interface IHealthDataSource
{
    string Name { get; }
    Task<object> GetHealthDataAsync(CancellationToken cancellationToken);
}