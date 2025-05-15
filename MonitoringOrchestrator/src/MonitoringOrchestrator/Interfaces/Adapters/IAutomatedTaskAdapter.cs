namespace TheSSS.DICOMViewer.Monitoring.Interfaces.Adapters;

using TheSSS.DICOMViewer.Monitoring.Contracts;

/// <summary>
/// Adapter interface for monitoring automated task statuses.
/// Implementation is expected to be provided by relevant REPO-APP-SERVICES that manage these tasks.
/// </summary>
public interface IAutomatedTaskAdapter
{
    /// <summary>
    /// Retrieves the status of key automated background tasks (e.g., data purge, backups).
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains an
    /// enumerable collection of AutomatedTaskStatusInfoDto, one for each monitored task.
    /// </returns>
    Task<IEnumerable<AutomatedTaskStatusInfoDto>> GetAutomatedTaskStatusesAsync(CancellationToken cancellationToken);
}