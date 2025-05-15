using TheSSS.DICOMViewer.Monitoring.Contracts;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TheSSS.DICOMViewer.Monitoring.Interfaces.Adapters;

/// <summary>
/// Adapter interface for retrieving PACS connectivity status.
/// This abstracts the retrieval of PACS status from underlying services (e.g., DicomNetworkService in REPO-APP-SERVICES).
/// </summary>
public interface IPacsStatusAdapter
{
    /// <summary>
    /// Retrieves the connectivity status for all configured PACS nodes.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains an enumerable collection of <see cref="PacsConnectionInfoDto"/>,
    /// each representing the status of a configured PACS node.
    /// </returns>
    Task<IEnumerable<PacsConnectionInfoDto>> GetAllPacsStatusesAsync(CancellationToken cancellationToken);
}