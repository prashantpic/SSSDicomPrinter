namespace TheSSS.DICOMViewer.Monitoring.Interfaces.Adapters;

using TheSSS.DICOMViewer.Monitoring.Contracts;

/// <summary>
/// Adapter interface for retrieving storage information.
/// Implementation is expected to be provided by REPO-INFRA or REPO-APP-SERVICES.
/// </summary>
public interface IStorageInfoAdapter
{
    /// <summary>
    /// Retrieves information about the local DICOM storage utilization.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains
    /// a StorageHealthInfoDto detailing total capacity, free space, and usage percentage.
    /// </returns>
    Task<StorageHealthInfoDto> GetStorageHealthInfoAsync(CancellationToken cancellationToken);
}