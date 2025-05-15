using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TheSSS.DICOMViewer.Monitoring.Contracts;

namespace TheSSS.DICOMViewer.Monitoring.Interfaces.Adapters;

/// <summary>
/// Adapter interface for monitoring automated task statuses (likely from relevant REPO-APP-SERVICES or REPO-ORCH-001).
/// </summary>
public interface IAutomatedTaskAdapter
{
    /// <summary>
    /// Retrieves the status of key automated background tasks.
    /// Implementations should query the respective services or schedulers that manage these tasks.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Task resolving to an enumerable collection of AutomatedTaskStatusInfoDto.
    /// Returns an empty collection if no tasks are configured for monitoring.
    /// </returns>
    /// <exception cref="TheSSS.DICOMViewer.Monitoring.Exceptions.DataSourceUnavailableException">
    /// Thrown if the adapter encounters a significant issue preventing retrieval of any task status.
    /// Issues with individual tasks should be reflected in their DTOs.
    /// </exception>
    Task<IEnumerable<AutomatedTaskStatusInfoDto>> GetAutomatedTaskStatusesAsync(CancellationToken cancellationToken);
}