using System.Threading;
using System.Threading.Tasks;

namespace TheSSS.DICOMViewer.Monitoring.Interfaces;

/// <summary>
/// Defines a contract for components that provide specific health data for a part of the system.
/// </summary>
public interface IHealthDataSource
{
    /// <summary>
    /// Asynchronously retrieves specific health data from the data source.
    /// The returned object should be a DTO specific to the health data being provided (e.g., StorageHealthInfoDto, PacsConnectionInfoDto).
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains an object
    /// representing the health data. Consumers will need to cast this to the expected DTO type.
    /// </returns>
    Task<object> GetHealthDataAsync(CancellationToken cancellationToken);
}