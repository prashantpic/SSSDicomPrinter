using TheSSS.DICOMViewer.Monitoring.Contracts;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TheSSS.DICOMViewer.Monitoring.Interfaces.Adapters;

/// <summary>
/// Adapter interface for monitoring automated task statuses.
/// This abstracts retrieval of automated task statuses (e.g., data purge, backups) from relevant application services.
/// </summary>
public interface IAutomatedTaskAdapter
{
    /// <summary>
    /// Retrieves the status of key automated background tasks.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains an enumerable collection of <see cref="AutomatedTaskStatusInfoDto"/>,
    /// each representing the status of an automated task.
    /// </returns>
    Task<IEnumerable<AutomatedTaskStatusInfoDto>> GetAutomatedTaskStatusesAsync(CancellationToken cancellationToken);
}