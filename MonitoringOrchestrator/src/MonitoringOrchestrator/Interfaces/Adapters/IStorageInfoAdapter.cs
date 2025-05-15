using TheSSS.DICOMViewer.Monitoring.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace TheSSS.DICOMViewer.Monitoring.Interfaces.Adapters;

/// <summary>
/// Adapter interface for retrieving storage information.
/// This abstracts the retrieval of local DICOM repository storage utilization from underlying infrastructure or application services.
/// </summary>
public interface IStorageInfoAdapter
{
    /// <summary>
    /// Retrieves information about the local DICOM storage utilization.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains a <see cref="StorageHealthInfoDto"/> with details about storage utilization.
    /// </returns>
    Task<StorageHealthInfoDto> GetStorageHealthInfoAsync(CancellationToken cancellationToken);
}