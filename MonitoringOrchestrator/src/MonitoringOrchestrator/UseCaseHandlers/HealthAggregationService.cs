using TheSSS.DICOMViewer.Monitoring.Interfaces;
using TheSSS.DICOMViewer.Monitoring.Contracts;
using TheSSS.DICOMViewer.Monitoring.Exceptions;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TheSSS.DICOMViewer.Monitoring.UseCaseHandlers;

public class HealthAggregationService
{
    private readonly IEnumerable<IHealthDataSource> _dataSources;
    private readonly ILogger<HealthAggregationService> _logger;

    public HealthAggregationService(
        IEnumerable<IHealthDataSource> dataSources,
        ILogger<HealthAggregationService> logger)
    {
        _dataSources = dataSources;
        _logger = logger;
    }

    public async Task<HealthReportDto> AggregateHealthAsync(CancellationToken cancellationToken)
    {
        var report = new HealthReportDto
        {
            Timestamp = DateTime.UtcNow,
            OverallStatus = OverallHealthStatus.Unknown
        };

        var tasks = _dataSources.Select(source => CollectDataSourceDataAsync(source, cancellationToken)).ToList();
        var results = await Task.WhenAll(tasks);

        foreach (var result in results)
        {
            if (result.Success)
            {
                switch (result.Data)
                {
                    case StorageHealthInfoDto storage:
                        report.StorageHealth = storage;
                        break;
                    case DatabaseConnectivityInfoDto db:
                        report.DatabaseHealth = db;
                        break;
                    case IEnumerable<PacsConnectionInfoDto> pacs:
                        report.PacsConnections = pacs.ToList();
                        break;
                    case LicenseStatusInfoDto license:
                        report.LicenseStatus = license;
                        break;
                    case SystemErrorInfoSummaryDto errors:
                        report.SystemErrorSummary = errors;
                        break;
                    case IEnumerable<AutomatedTaskStatusInfoDto> tasks:
                        report.AutomatedTaskStatuses = tasks.ToList();
                        break;
                }
            }
        }

        report.OverallStatus = DetermineOverallStatus(report);
        return report;
    }

    private async Task<(bool Success, object? Data, string SourceName)> CollectDataSourceDataAsync(
        IHealthDataSource source, CancellationToken cancellationToken)
    {
        try
        {
            var data = await source.GetHealthDataAsync(cancellationToken);
            return (true, data, source.Name);
        }
        catch (DataSourceUnavailableException ex)
        {
            _logger.LogWarning(ex, "Data source {DataSource} unavailable", source.Name);
            return (false, null, source.Name);
        }
    }

    private static OverallHealthStatus DetermineOverallStatus(HealthReportDto report)
    {
        if (report.DatabaseHealth?.IsConnected == false) return OverallHealthStatus.Error;
        if (report.LicenseStatus?.IsValid == false) return OverallHealthStatus.Error;
        if (report.StorageHealth?.UsedPercentage > 90) return OverallHealthStatus.Warning;
        if (report.PacsConnections?.Any(p => !p.IsConnected) == true) return OverallHealthStatus.Warning;
        if (report.SystemErrorSummary?.CriticalErrorCountLast24Hours > 0) return OverallHealthStatus.Critical;
        
        return OverallHealthStatus.Healthy;
    }
}