using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TheSSS.DICOMViewer.Monitoring.Contracts;
using TheSSS.DICOMViewer.Monitoring.Exceptions;
using TheSSS.DICOMViewer.Monitoring.Interfaces;
using TheSSS.DICOMViewer.Monitoring.Interfaces.Adapters; // Assuming ILoggerAdapter
using System.Linq; // Added for .Count()

namespace TheSSS.DICOMViewer.Monitoring.HealthDataSources;

public class PacsStatusDataSource : IHealthDataSource
{
    private readonly IPacsStatusAdapter _pacsStatusAdapter;
    private readonly ILoggerAdapter<PacsStatusDataSource> _logger;

    public PacsStatusDataSource(IPacsStatusAdapter pacsStatusAdapter, ILoggerAdapter<PacsStatusDataSource> logger)
    {
        _pacsStatusAdapter = pacsStatusAdapter;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<object> GetHealthDataAsync(CancellationToken cancellationToken)
    {
        _logger.Debug("Collecting PACS status data via adapter.");
        try
        {
            var statuses = await _pacsStatusAdapter.GetAllPacsStatusesAsync(cancellationToken);
            _logger.Debug($"Successfully collected PACS status data for {statuses.Count()} nodes.");
            return statuses; // Return IEnumerable<PacsConnectionInfoDto>
        }
         catch (DataSourceUnavailableException ex)
        {
            _logger.Error(ex, "PACS status data source adapter reported unavailable.");
            throw; // Re-throw DataSourceUnavailableException
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "An unexpected error occurred while getting PACS status data.");
            throw new DataSourceUnavailableException(nameof(PacsStatusDataSource), "Failed to retrieve PACS status data due to an internal error.", ex);
        }
    }
}