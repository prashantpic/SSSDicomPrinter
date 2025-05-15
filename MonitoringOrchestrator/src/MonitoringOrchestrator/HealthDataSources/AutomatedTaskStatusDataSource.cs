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

public class AutomatedTaskStatusDataSource : IHealthDataSource
{
    private readonly IAutomatedTaskAdapter _automatedTaskAdapter;
    private readonly ILoggerAdapter<AutomatedTaskStatusDataSource> _logger;

    public AutomatedTaskStatusDataSource(IAutomatedTaskAdapter automatedTaskAdapter, ILoggerAdapter<AutomatedTaskStatusDataSource> logger)
    {
        _automatedTaskAdapter = automatedTaskAdapter;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<object> GetHealthDataAsync(CancellationToken cancellationToken)
    {
        _logger.Debug("Collecting automated task status data via adapter.");
        try
        {
            var statuses = await _automatedTaskAdapter.GetAutomatedTaskStatusesAsync(cancellationToken);
            _logger.Debug($"Successfully collected automated task status data for {statuses.Count()} tasks.");
            return statuses; // Return IEnumerable<AutomatedTaskStatusInfoDto>
        }
        catch (DataSourceUnavailableException ex)
        {
            _logger.Error(ex, "Automated task status data source adapter reported unavailable.");
            throw; // Re-throw DataSourceUnavailableException
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "An unexpected error occurred while getting automated task status data.");
            throw new DataSourceUnavailableException(nameof(AutomatedTaskStatusDataSource), "Failed to retrieve automated task status data due to an internal error.", ex);
        }
    }
}