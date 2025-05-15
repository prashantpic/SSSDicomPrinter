using Microsoft.Extensions.Logging;
using TheSSS.DICOMViewer.Monitoring.Interfaces;
using TheSSS.DICOMViewer.Monitoring.Interfaces.Adapters;
using TheSSS.DICOMViewer.Monitoring.Exceptions;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace TheSSS.DICOMViewer.Monitoring.HealthDataSources
{
    /// <summary>
    /// Implementation of <see cref="IHealthDataSource"/> for monitoring PACS connectivity.
    /// Provides health information related to PACS node connectivity by checking all configured PACS nodes.
    /// </summary>
    public class PacsStatusDataSource : IHealthDataSource
    {
        private readonly IPacsStatusAdapter _pacsStatusAdapter;
        private readonly ILogger<PacsStatusDataSource> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="PacsStatusDataSource"/> class.
        /// </summary>
        /// <param name="pacsStatusAdapter">The adapter for retrieving PACS statuses.</param>
        /// <param name="logger">The logger.</param>
        public PacsStatusDataSource(
            IPacsStatusAdapter pacsStatusAdapter,
            ILogger<PacsStatusDataSource> logger)
        {
            _pacsStatusAdapter = pacsStatusAdapter ?? throw new ArgumentNullException(nameof(pacsStatusAdapter));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc/>
        public async Task<object> GetHealthDataAsync(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogDebug("Fetching PACS node statuses.");
                var pacsStatuses = await _pacsStatusAdapter.GetAllPacsStatusesAsync(cancellationToken);
                _logger.LogDebug("Successfully fetched {Count} PACS node statuses.", pacsStatuses?.Count() ?? 0);
                return pacsStatuses ?? Enumerable.Empty<Contracts.PacsConnectionInfoDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve PACS node statuses.");
                throw new DataSourceUnavailableException("Failed to retrieve PACS node statuses.", ex, nameof(PacsStatusDataSource));
            }
        }
    }
}