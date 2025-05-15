namespace TheSSS.DICOMViewer.Monitoring.Interfaces.Adapters;

using TheSSS.DICOMViewer.Monitoring.Contracts;

/// <summary>
/// Adapter interface for checking database connectivity.
/// Implementation is expected to be provided by REPO-INFRA (e.g., a data access component).
/// </summary>
public interface IDbConnectivityAdapter
{
    /// <summary>
    /// Checks the connectivity to the application database (e.g., SQLite).
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains
    /// a DatabaseConnectivityInfoDto indicating connection status, latency, and any errors.
    /// </returns>
    Task<DatabaseConnectivityInfoDto> CheckDatabaseConnectivityAsync(CancellationToken cancellationToken);
}