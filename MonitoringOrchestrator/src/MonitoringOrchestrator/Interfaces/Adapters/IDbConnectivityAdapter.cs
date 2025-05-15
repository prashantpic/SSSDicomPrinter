using System.Threading;
using System.Threading.Tasks;
using TheSSS.DICOMViewer.Monitoring.Contracts;

namespace TheSSS.DICOMViewer.Monitoring.Interfaces.Adapters;

/// <summary>
/// Adapter interface for checking database connectivity (likely from REPO-INFRA).
/// Focuses on the application's primary database (e.g., SQLite).
/// </summary>
public interface IDbConnectivityAdapter
{
    /// <summary>
    /// Checks the connectivity to the application database.
    /// Implementations should attempt a lightweight operation like opening a connection or a simple query.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Task resolving to the DatabaseConnectivityInfoDto, indicating success/failure and optional latency.</returns>
    /// <exception cref="TheSSS.DICOMViewer.Monitoring.Exceptions.DataSourceUnavailableException">
    /// Thrown if the adapter itself encounters a critical error preventing the check (e.g., configuration issue),
    /// not just if the database is down (which should be reported in the DTO).
    /// </exception>
    Task<DatabaseConnectivityInfoDto> CheckDatabaseConnectivityAsync(CancellationToken cancellationToken);
}