using Microsoft.Extensions.Logging;
using TheSSS.DICOMViewer.Monitoring.Interfaces;
using TheSSS.DICOMViewer.Monitoring.Interfaces.Adapters;
using TheSSS.DICOMViewer.Monitoring.Exceptions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace TheSSS.DICOMViewer.Monitoring.HealthDataSources
{
    /// <summary>
    /// Implementation of <see cref="IHealthDataSource"/> for monitoring license status.
    /// Provides health information related to application license status.
    /// </summary>
    public class LicenseStatusDataSource : IHealthDataSource
    {
        private readonly ILicenseStatusAdapter _licenseStatusAdapter;
        private readonly ILogger<LicenseStatusDataSource> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="LicenseStatusDataSource"/> class.
        /// </summary>
        /// <param name="licenseStatusAdapter">The adapter for retrieving license status.</param>
        /// <param name="logger">The logger.</param>
        public LicenseStatusDataSource(
            ILicenseStatusAdapter licenseStatusAdapter,
            ILogger<LicenseStatusDataSource> logger)
        {
            _licenseStatusAdapter = licenseStatusAdapter ?? throw new ArgumentNullException(nameof(licenseStatusAdapter));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc/>
        public async Task<object> GetHealthDataAsync(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogDebug("Fetching license status.");
                var licenseStatus = await _licenseStatusAdapter.GetLicenseStatusAsync(cancellationToken);
                _logger.LogDebug("Successfully fetched license status. IsValid: {IsValid}", licenseStatus.IsValid);
                return licenseStatus;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve license status.");
                throw new DataSourceUnavailableException("Failed to retrieve license status.", ex, nameof(LicenseStatusDataSource));
            }
        }
    }
}