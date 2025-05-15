using TheSSS.DICOMViewer.Monitoring.Interfaces;
using TheSSS.DICOMViewer.Monitoring.Interfaces.Adapters;
using TheSSS.DICOMViewer.Monitoring.Exceptions;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TheSSS.DICOMViewer.Monitoring.HealthDataSources
{
    public class PacsStatusDataSource : IHealthDataSource
    {
        private readonly IPacsStatusAdapter _pacsStatusAdapter;
        private readonly ILogger<PacsStatusDataSource> _logger;

        public string Name => "PACSConnectivity";

        public PacsStatusDataSource(
            IPacsStatusAdapter pacsStatusAdapter,
            ILogger<PacsStatusDataSource> logger)
        {
            _pacsStatusAdapter = pacsStatusAdapter ?? throw new ArgumentNullException(nameof(pacsStatusAdapter));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<object> GetHealthDataAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("Attempting to retrieve PACS statuses for data source: {DataSourceName}.", Name);
            try
            {
                var pacsStatuses = await _pacsStatusAdapter.GetAllPacsStatusesAsync(cancellationToken);
                var statusesList = pacsStatuses.ToList();
                _logger.LogInformation("Successfully retrieved {Count} PACS statuses for data source: {DataSourceName}.", statusesList.Count, Name);
                return statusesList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving PACS statuses for data source: {DataSourceName}.", Name);
                throw new DataSourceUnavailableException(Name, $"Failed to retrieve PACS statuses due to: {ex.Message}", ex);
            }
        }
    }
}