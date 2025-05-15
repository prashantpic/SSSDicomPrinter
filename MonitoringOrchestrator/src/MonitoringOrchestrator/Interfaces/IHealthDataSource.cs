namespace TheSSS.DICOMViewer.Monitoring.Interfaces;

/// <summary>
/// Interface for components that provide specific health data.
/// </summary>
public interface IHealthDataSource
{
    /// <summary>
    /// Gets a descriptive name for the data source (e.g., "Storage", "Database", "PACSConnectivity").
    /// This can be used for logging and identifying the source of data in aggregated reports.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Asynchronously retrieves specific health data.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains an object
    /// representing the health data DTO (e.g., StorageHealthInfoDto, List<PacsConnectionInfoDto>).
    /// The implementing class should return its specific DTO type, cast to object.
    /// </returns>
    Task<object> GetHealthDataAsync(CancellationToken cancellationToken);
}