using System.Threading;
using System.Threading.Tasks;
using TheSSS.DICOMViewer.Monitoring.Contracts;

namespace TheSSS.DICOMViewer.Monitoring.Interfaces.Adapters;

/// <summary>
/// Adapter interface for retrieving storage information (likely from REPO-INFRA or REPO-APP-SERVICES).
/// Focuses on the local DICOM repository storage.
/// </summary>
public interface IStorageInfoAdapter
{
    /// <summary>
    /// Retrieves information about the local DICOM storage utilization.
    /// The implementation should target the primary storage location configured for DICOM files.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Task resolving to the StorageHealthInfoDto.</returns>
    /// <exception cref="TheSSS.DICOMViewer.Monitoring.Exceptions.DataSourceUnavailableException">
    /// Thrown if the adapter cannot retrieve storage information (e.g., path not found, permission issues).
    /// </exception>
    Task<StorageHealthInfoDto> GetStorageHealthInfoAsync(CancellationToken cancellationToken);
}