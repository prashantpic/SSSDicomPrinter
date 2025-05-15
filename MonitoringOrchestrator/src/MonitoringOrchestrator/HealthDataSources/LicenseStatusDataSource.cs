using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TheSSS.DICOMViewer.Monitoring.Contracts;
using TheSSS.DICOMViewer.Monitoring.Exceptions;
using TheSSS.DICOMViewer.Monitoring.Interfaces;
using TheSSS.DICOMViewer.Monitoring.Interfaces.Adapters; // Assuming ILoggerAdapter

namespace TheSSS.DICOMViewer.Monitoring.HealthDataSources;

public class LicenseStatusDataSource : IHealthDataSource
{
    private readonly ILicenseStatusAdapter _licenseStatusAdapter;
    private readonly ILoggerAdapter<LicenseStatusDataSource> _logger;

    public LicenseStatusDataSource(ILicenseStatusAdapter licenseStatusAdapter, ILoggerAdapter<LicenseStatusDataSource> logger)
    {
        _licenseStatusAdapter = licenseStatusAdapter;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<object> GetHealthDataAsync(CancellationToken cancellationToken)
    {
        _logger.Debug("Collecting license status data via adapter.");
        try
        {
            var status = await _licenseStatusAdapter.GetLicenseStatusAsync(cancellationToken);
            _logger.Debug($"Successfully collected license status data. IsValid: {status.IsValid}");
            return status; // Return LicenseStatusInfoDto
        }
        catch (DataSourceUnavailableException ex)
        {
            _logger.Error(ex, "License status data source adapter reported unavailable.");
            throw; // Re-throw DataSourceUnavailableException
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "An unexpected error occurred while getting license status data.");
            throw new DataSourceUnavailableException(nameof(LicenseStatusDataSource), "Failed to retrieve license status data due to an internal error.", ex);
        }
    }
}