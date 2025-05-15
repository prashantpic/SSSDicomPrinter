namespace TheSSS.DICOMViewer.Monitoring.Interfaces.Adapters;

public interface ILicenseStatusAdapter
{
    Task<LicenseStatusInfoDto> GetLicenseStatusAsync(CancellationToken cancellationToken);
}