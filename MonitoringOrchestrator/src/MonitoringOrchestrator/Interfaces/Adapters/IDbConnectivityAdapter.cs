using TheSSS.DICOMViewer.Monitoring.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace TheSSS.DICOMViewer.Monitoring.Interfaces.Adapters;

/// <summary>
/// Adapter interface for checking database connectivity.
/// This abstracts the checking of SQLite database connection status, likely from an infrastructure component.
/// </summary>
public interface IDbConnectivityAdapter
{
    /// <summary>
    /// Checks the connectivity to the application database.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains a <see cref="DatabaseConnectivityInfoDto"/> with details about the database connection status.
    /// </returns>
    Task<DatabaseConnectivityInfoDto> CheckDatabaseConnectivityAsync(CancellationToken cancellationToken);
}