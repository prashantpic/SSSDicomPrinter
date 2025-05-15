using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TheSSS.DICOMViewer.Monitoring.Contracts;

namespace TheSSS.DICOMViewer.Monitoring.Interfaces.Adapters;

/// <summary>
/// Adapter interface for retrieving PACS connectivity status (likely from REPO-APP-SERVICES or other orchestrators).
/// </summary>
public interface IPacsStatusAdapter
{
    /// <summary>
    /// Retrieves the connectivity status for all configured PACS nodes.
    /// Implementations should perform C-ECHO or similar checks.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Task resolving to an enumerable collection of PacsConnectionInfoDto.
    /// Returns an empty collection if no PACS nodes are configured.
    /// Individual PacsConnectionInfoDto objects should reflect the status of each node.
    /// </returns>
    /// <exception cref="TheSSS.DICOMViewer.Monitoring.Exceptions.DataSourceUnavailableException">
    /// Thrown if the adapter encounters a significant issue preventing retrieval of any PACS status (e.g., underlying service is entirely down).
    /// Minor issues with individual PACS nodes should be reflected in their respective DTOs, not by throwing this exception.
    /// </exception>
    Task<IEnumerable<PacsConnectionInfoDto>> GetAllPacsStatusesAsync(CancellationToken cancellationToken);
}