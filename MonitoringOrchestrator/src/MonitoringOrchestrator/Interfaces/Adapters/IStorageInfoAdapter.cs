namespace TheSSS.DICOMViewer.Monitoring.Interfaces.Adapters;

public interface IStorageInfoAdapter
{
    Task<StorageHealthInfoDto> GetStorageHealthInfoAsync(CancellationToken cancellationToken);
}