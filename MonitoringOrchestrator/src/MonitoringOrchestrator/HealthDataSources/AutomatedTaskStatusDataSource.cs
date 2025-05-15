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
    /// Implementation of <see cref="IHealthDataSource"/> for monitoring automated task statuses.
    /// Provides status information for key automated tasks such as data purging or backups.
    /// </summary>
    public class AutomatedTaskStatusDataSource : IHealthDataSource
    {
        private readonly IAutomatedTaskAdapter _automatedTaskAdapter;
        private readonly ILogger<AutomatedTaskStatusDataSource> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutomatedTaskStatusDataSource"/> class.
        /// </summary>
        /// <param name="automatedTaskAdapter">The adapter for retrieving automated task statuses.</param>
        /// <param name="logger">The logger.</param>
        public AutomatedTaskStatusDataSource(
            IAutomatedTaskAdapter automatedTaskAdapter,
            ILogger<AutomatedTaskStatusDataSource> logger)
        {
            _automatedTaskAdapter = automatedTaskAdapter ?? throw new ArgumentNullException(nameof(automatedTaskAdapter));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc/>
        public async Task<object> GetHealthDataAsync(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogDebug("Fetching automated task statuses.");
                var taskStatuses = await _automatedTaskAdapter.GetAutomatedTaskStatusesAsync(cancellationToken);
                _logger.LogDebug("Successfully fetched {Count} automated task statuses.", taskStatuses?.Count() ?? 0);
                return taskStatuses ?? Enumerable.Empty<Contracts.AutomatedTaskStatusInfoDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve automated task statuses.");
                throw new DataSourceUnavailableException("Failed to retrieve automated task statuses.", ex, nameof(AutomatedTaskStatusDataSource));
            }
        }
    }
}