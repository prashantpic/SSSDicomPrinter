namespace TheSSS.DICOMViewer.Monitoring.Interfaces.Adapters;

using TheSSS.DICOMViewer.Monitoring.Contracts;

/// <summary>
/// Adapter interface for retrieving PACS connectivity status.
/// Implementation is expected to be provided by REPO-APP-SERVICES (e.g., a DicomNetworkService).
/// </summary>
public interface IPacsStatusAdapter
{
    /// <summary>
    /// Retrieves the connectivity status for all configured PACS nodes.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains an
    /// enumerable collection of PacsConnectionInfoDto, one for each configured PACS node.
    /// </returns>
    Task<IEnumerable<PacsConnectionInfoDto>> GetAllPacsStatusesAsync(CancellationToken cancellationToken);
}