using TheSSS.DICOMViewer.Monitoring.Interfaces;
using TheSSS.DICOMViewer.Monitoring.Interfaces.Adapters;
using TheSSS.DICOMViewer.Monitoring.Exceptions;
using Microsoft.Extensions.Logging;

namespace TheSSS.DICOMViewer.Monitoring.HealthDataSources;

public class LicenseStatusDataSource : IHealthDataSource
{
    private readonly ILicenseStatusAdapter _adapter;
    private readonly ILogger<LicenseStatusDataSource> _logger;
    public string Name => "License";

    public LicenseStatusDataSource(ILicenseStatusAdapter adapter, ILogger<LicenseStatusDataSource> logger)
        => (_adapter, _logger) = (adapter, logger);

    public async Task<object> GetHealthDataAsync(CancellationToken cancellationToken)
    {
        try
        {
            return await _adapter.GetLicenseStatusAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "License status check failed");
            throw new DataSourceUnavailableException(Name, ex.Message, ex);
        }
    }
}