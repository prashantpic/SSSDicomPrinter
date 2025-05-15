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

        var dataTasks = _dataSources.Select(CollectDataSourceData);
        var results = await Task.WhenAll(dataTasks);

        foreach (var result in results)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing data from {DataSource}", result.SourceName);
            }
        }

        report.OverallStatus = DetermineOverallStatus(report);
        return report;
    }

    private async Task<(string SourceName, object Data)> CollectDataSourceData(IHealthDataSource source)
    {
        try
        {
            var data = await source.GetHealthDataAsync(CancellationToken.None);
            return (source.Name, data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to collect data from {DataSource}", source.Name);
            throw new DataSourceUnavailableException(source.Name, ex.Message, ex);
        }
    }

    private static OverallHealthStatus DetermineOverallStatus(HealthReportDto report)
    {
        if (report.DatabaseHealth?.IsConnected == false) return OverallHealthStatus.Error;
        if (report.LicenseStatus?.IsValid == false) return OverallHealthStatus.Error;
        if (report.StorageHealth?.UsedPercentage >= 90) return OverallHealthStatus.Warning;
        if (report.PacsConnections?.Any(p => !p.IsConnected) == true) return OverallHealthStatus.Warning;
        return report.SystemErrorSummary?.CriticalErrorCountLast24Hours > 0 
            ? OverallHealthStatus.Error 
            : OverallHealthStatus.Healthy;
    }
}