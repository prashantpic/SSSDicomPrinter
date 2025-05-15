namespace TheSSS.DICOMViewer.Monitoring.Interfaces.Adapters;

public interface IDbConnectivityAdapter
{
    Task<DatabaseConnectivityInfoDto> CheckDatabaseConnectivityAsync(CancellationToken cancellationToken);
}