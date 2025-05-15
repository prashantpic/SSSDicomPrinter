namespace TheSSS.DICOMViewer.Monitoring.Interfaces.Adapters;

using System.Threading;
using System.Threading.Tasks;
using TheSSS.DICOMViewer.Monitoring.Contracts;

public interface IStorageInfoAdapter
{
    Task<StorageHealthInfoDto> GetStorageHealthInfoAsync(CancellationToken cancellationToken);
}