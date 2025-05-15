namespace TheSSS.DICOMViewer.Monitoring.Interfaces.Adapters;

using System.Threading;
using System.Threading.Tasks;
using TheSSS.DICOMViewer.Monitoring.Contracts;

public interface IDbConnectivityAdapter
{
    Task<DatabaseConnectivityInfoDto> CheckDatabaseConnectivityAsync(CancellationToken cancellationToken);
}