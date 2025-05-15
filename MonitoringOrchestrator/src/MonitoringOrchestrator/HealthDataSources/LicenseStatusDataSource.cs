using TheSSS.DICOMViewer.Monitoring.Interfaces;
using TheSSS.DICOMViewer.Monitoring.Interfaces.Adapters;
using TheSSS.DICOMViewer.Monitoring.Exceptions;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace TheSSS.DICOMViewer.Monitoring.HealthDataSources
{
    public class LicenseStatusDataSource : IHealthDataSource
    {
        private readonly ILicenseStatusAdapter _licenseStatusAdapter;
        private readonly ILogger<LicenseStatusDataSource> _logger;

        public string Name => "License";

        public LicenseStatusDataSource(
            ILicenseStatusAdapter licenseStatusAdapter,
            ILogger<LicenseStatusDataSource> logger)
        {
            _licenseStatusAdapter = licenseStatusAdapter ?? throw new ArgumentNullException(nameof(licenseStatusAdapter));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<object> GetHealthDataAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("Attempting to retrieve license status for data source: {DataSourceName}.", Name);
            try
            {
                var licenseStatus = await _licenseStatusAdapter.GetLicenseStatusAsync(cancellationToken);
                _logger.LogInformation("Successfully retrieved license status for data source: {DataSourceName}. IsValid: {IsValid}, DaysUntilExpiry: {DaysUntilExpiry}.", Name, licenseStatus.IsValid, licenseStatus.DaysUntilExpiry?.ToString() ?? "N/A");
                return licenseStatus;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving license status for data source: {DataSourceName}.", Name);
                throw new DataSourceUnavailableException(Name, $"Failed to retrieve license status due to: {ex.Message}", ex);
            }
        }
    }
}