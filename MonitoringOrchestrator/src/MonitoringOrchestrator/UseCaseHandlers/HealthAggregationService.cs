namespace TheSSS.DICOMViewer.Monitoring.UseCaseHandlers;

using TheSSS.DICOMViewer.Monitoring.Interfaces;
using TheSSS.DICOMViewer.Monitoring.Contracts;
using TheSSS.DICOMViewer.Monitoring.Mappers;
using TheSSS.DICOMViewer.Monitoring.Exceptions;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System;

public class HealthAggregationService
{
    private readonly IEnumerable<IHealthDataSource> _dataSources;
    private readonly ILogger<HealthAggregationService> _logger;

    public HealthAggregationService(
        IEnumerable<IHealthDataSource> dataSources,
        ILogger<HealthAggregationService> logger)
    {
        _dataSources = dataSources ?? throw new ArgumentNullException(nameof(dataSources));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Collects health data from all registered data sources and aggregates it into a HealthReportDto.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation, returning the aggregated HealthReportDto.</returns>
    public async Task<HealthReportDto> AggregateHealthDataAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting health data aggregation.");

        var report = new HealthReportDto
        {
            Timestamp = DateTime.UtcNow,
            PacsConnections = new List<PacsConnectionInfoDto>(),
            AutomatedTaskStatuses = new List<AutomatedTaskStatusInfoDto>()
        };

        var tasks = _dataSources.Select(source => CollectDataSourceDataAsync(source, cancellationToken)).ToList();
        
        // Using Task.WhenAll to run data collection concurrently
        var results = await Task.WhenAll(tasks);

        // Process results and populate the report DTO
        foreach (var result in results)
        {
            if (result.Success && result.Data != null)
            {
                switch (result.Data)
                {
                    case StorageHealthInfoDto storageHealth:
                        report.StorageHealth = storageHealth;
                        _logger.LogDebug("Aggregated StorageHealth data.");
                        break;
                    case DatabaseConnectivityInfoDto databaseHealth:
                        report.DatabaseHealth = databaseHealth;
                        _logger.LogDebug("Aggregated DatabaseHealth data.");
                        break;
                    case IEnumerable<PacsConnectionInfoDto> pacsConnections:
                        report.PacsConnections = pacsConnections;
                        _logger.LogDebug($"Aggregated {pacsConnections.Count()} PACSConnectionInfo data points.");
                        break;
                    case PacsConnectionInfoDto singlePacsConnection: // If a source returns a single Pacs item
                        ((List<PacsConnectionInfoDto>)report.PacsConnections!).Add(singlePacsConnection);
                         _logger.LogDebug($"Aggregated single PACSConnectionInfo data for {singlePacsConnection.PacsNodeId}.");
                        break;
                    case LicenseStatusInfoDto licenseStatus:
                        report.LicenseStatus = licenseStatus;
                        _logger.LogDebug("Aggregated LicenseStatus data.");
                        break;
                    case SystemErrorInfoSummaryDto systemErrorSummary:
                        report.SystemErrorSummary = systemErrorSummary;
                        _logger.LogDebug("Aggregated SystemErrorSummary data.");
                        break;
                    case IEnumerable<AutomatedTaskStatusInfoDto> taskStatuses:
                        report.AutomatedTaskStatuses = taskStatuses;
                        _logger.LogDebug($"Aggregated {taskStatuses.Count()} AutomatedTaskStatusInfo data points.");
                        break;
                    case AutomatedTaskStatusInfoDto singleTaskStatus: // If a source returns a single Task item
                        ((List<AutomatedTaskStatusInfoDto>)report.AutomatedTaskStatuses!).Add(singleTaskStatus);
                        _logger.LogDebug($"Aggregated single AutomatedTaskStatusInfo data for {singleTaskStatus.TaskName}.");
                        break;
                    default:
                        _logger.LogWarning($"Aggregator received data from source '{result.DataSourceName}' of unhandled type: {result.Data.GetType().FullName}");
                        break;
                }
            }
            else if (!result.Success)
            {
                _logger.LogWarning($"Data collection failed for source '{result.DataSourceName}'. Error: {result.Error?.Message}");
                // Optionally, add specific error indicators to the HealthReportDto for failed sources
            }
        }

        // Determine overall status based on collected data
        report.OverallStatus = HealthReportMapper.DetermineOverallStatus(report);

        _logger.LogInformation($"Health data aggregation finished. Overall status: {report.OverallStatus}");

        return report;
    }

    private async Task<(string DataSourceName, bool Success, object? Data, Exception? Error)> CollectDataSourceDataAsync(
        IHealthDataSource source,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogDebug($"Collecting data from source: {source.Name}");
            var data = await source.GetHealthDataAsync(cancellationToken);
            _logger.LogDebug($"Successfully collected data from source: {source.Name}");
            return (source.Name, true, data, null);
        }
        catch (DataSourceUnavailableException dsuEx)
        {
            _logger.LogError(dsuEx, $"Data source '{source.Name}' is unavailable.");
            return (source.Name, false, null, dsuEx);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An unexpected error occurred while collecting data from source '{source.Name}'.");
            return (source.Name, false, null, ex);
        }
    }
}