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
    public class AutomatedTaskStatusDataSource : IHealthDataSource
    {
        private readonly IAutomatedTaskAdapter _automatedTaskAdapter;
        private readonly ILogger<AutomatedTaskStatusDataSource> _logger;

        public string Name => "AutomatedTasks";

        public AutomatedTaskStatusDataSource(
            IAutomatedTaskAdapter automatedTaskAdapter,
            ILogger<AutomatedTaskStatusDataSource> logger)
        {
            _automatedTaskAdapter = automatedTaskAdapter ?? throw new ArgumentNullException(nameof(automatedTaskAdapter));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<object> GetHealthDataAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("Attempting to retrieve automated task statuses for data source: {DataSourceName}.", Name);
            try
            {
                var taskStatuses = await _automatedTaskAdapter.GetAutomatedTaskStatusesAsync(cancellationToken);
                var statusesList = taskStatuses.ToList();
                _logger.LogInformation("Successfully retrieved {Count} automated task statuses for data source: {DataSourceName}.", statusesList.Count, Name);
                return statusesList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving automated task statuses for data source: {DataSourceName}.", Name);
                throw new DataSourceUnavailableException(Name, $"Failed to retrieve automated task statuses due to: {ex.Message}", ex);
            }
        }
    }
}