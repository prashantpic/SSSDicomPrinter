using System.Threading;
using System.Threading.Tasks;

namespace TheSSS.DICOMViewer.Monitoring.Interfaces;

/// <summary>
/// Interface for components that provide specific health data.
/// Implementations should return a Task<object> where the object is a specific
/// health DTO (e.g., StorageHealthInfoDto, DatabaseConnectivityInfoDto, etc.).
/// </summary>
public interface IHealthDataSource
{
    /// <summary>
    /// Asynchronously retrieves specific health data.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Task resolving to an object representing the health data (e.g., a DTO).
    /// The object returned must match one of the expected DTO types for processing in HealthAggregationService.
    /// </returns>
    /// <exception cref="TheSSS.DICOMViewer.Monitoring.Exceptions.DataSourceUnavailableException">
    /// Thrown if the data source cannot retrieve data due to an issue with the underlying source (e.g., service down, permissions).
    /// </exception>
    /// <exception cref="System.OperationCanceledException">
    /// Thrown if the operation is cancelled via the cancellation token.
    /// </exception>
    Task<object> GetHealthDataAsync(CancellationToken cancellationToken);
}