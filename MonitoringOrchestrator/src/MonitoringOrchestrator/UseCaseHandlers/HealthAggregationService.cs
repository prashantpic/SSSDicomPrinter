using TheSSS.DICOMViewer.Monitoring.Contracts;
using TheSSS.DICOMViewer.Monitoring.Exceptions;
using TheSSS.DICOMViewer.Monitoring.Interfaces;
using TheSSS.DICOMViewer.Monitoring.Interfaces.Adapters;

namespace TheSSS.DICOMViewer.Monitoring.UseCaseHandlers;

public class HealthAggregationService
{
    private readonly ILoggerAdapter<HealthAggregationService> _logger;
    private readonly IEnumerable<IHealthDataSource> _dataSources;
    private readonly PrometheusMetricsCollector? _metricsCollector; // Optional, if direct update is preferred

    private enum HealthLevel { Healthy = 0, Warning = 1, Degraded = 2, Error = 3 }

    public HealthAggregationService(
        ILoggerAdapter<HealthAggregationService> logger,
        IEnumerable<IHealthDataSource> dataSources,
        PrometheusMetricsCollector? metricsCollector = null) // Make metricsCollector optional
    {
        _logger = logger;
        _dataSources = dataSources ?? throw new ArgumentNullException(nameof(dataSources));
        _metricsCollector = metricsCollector;
    }

    public async Task<HealthReportDto> AggregateHealthDataAsync(CancellationToken cancellationToken)
    {
        _logger.Info("Starting health data aggregation cycle.");
        var report = new HealthReportDto
        {
            Timestamp = DateTimeOffset.UtcNow,
            // OverallStatus will be determined based on aggregated data
        };

        var currentOverallHealthLevel = HealthLevel.Healthy;

        var collectionTasks = _dataSources.Select(source => CollectDataFromSourceAsync(source, cancellationToken)).ToList();
        
        var results = await Task.WhenAll(collectionTasks);

        foreach (var result in results)
        {
            if (!result.Success || result.DataObject == null)
            {
                // If a data source fails, consider the system Degraded.
                // Critical failures might elevate this to Error depending on policy.
                currentOverallHealthLevel = UpdateHealthLevel(currentOverallHealthLevel, HealthLevel.Degraded);
                _logger.Warning($"Data source '{result.SourceType}' failed to provide data or returned null.");
                report.AdditionalData.TryAdd($"Error_{result.SourceType}", $"Data source '{result.SourceType}' failed or returned null.");
                continue;
            }

            object data = result.DataObject;
            HealthLevel itemHealthLevel = HealthLevel.Healthy; // Assume healthy for this item

            switch (data)
            {
                case StorageHealthInfoDto storageInfo:
                    report.StorageInfo = storageInfo;
                    if (storageInfo.UsedPercentage > 95) itemHealthLevel = HealthLevel.Error;
                    else if (storageInfo.UsedPercentage > 90) itemHealthLevel = HealthLevel.Warning;
                    _logger.Debug($"Aggregated Storage Info: Identifier='{storageInfo.StorageIdentifier}', Used={storageInfo.UsedPercentage:F1}%. Item Health: {itemHealthLevel}");
                    break;
                case DatabaseConnectivityInfoDto dbInfo:
                    report.DatabaseConnectivity = dbInfo;
                    if (!dbInfo.IsConnected) itemHealthLevel = HealthLevel.Error;
                    else if (dbInfo.LatencyMs > 1000) itemHealthLevel = HealthLevel.Warning; // Example latency warning
                    _logger.Debug($"Aggregated Database Info: Connected={dbInfo.IsConnected}, Latency={dbInfo.LatencyMs}ms. Item Health: {itemHealthLevel}");
                    break;
                case IEnumerable<PacsConnectionInfoDto> pacsInfos:
                    report.PacsConnections = pacsInfos.ToList();
                    if (pacsInfos.Any(p => !p.IsConnected))
                    {
                        // If any PACS node is critical (e.g., based on consecutive failures if that data was available) -> Error
                        // For now, any disconnected PACS is a Warning, multiple or persistent could be Error.
                        itemHealthLevel = HealthLevel.Warning; 
                        if (pacsInfos.Count(p => !p.IsConnected) > 1 || pacsInfos.Any(p => !p.IsConnected && (DateTimeOffset.UtcNow - (p.LastFailedEchoTimestamp ?? DateTimeOffset.MinValue)).TotalMinutes > 30))
                        {
                             itemHealthLevel = HealthLevel.Error; // Example: multiple failures or prolonged failure
                        }
                    }
                    _logger.Debug($"Aggregated PACS Info: {pacsInfos.Count()} nodes. Disconnected: {pacsInfos.Count(p => !p.IsConnected)}. Item Health: {itemHealthLevel}");
                    break;
                case LicenseStatusInfoDto licenseInfo:
                    report.LicenseStatus = licenseInfo;
                    if (!licenseInfo.IsValid) itemHealthLevel = HealthLevel.Error;
                    else if (licenseInfo.DaysUntilExpiry.HasValue && licenseInfo.DaysUntilExpiry.Value < 7) itemHealthLevel = HealthLevel.Error;
                    else if (licenseInfo.DaysUntilExpiry.HasValue && licenseInfo.DaysUntilExpiry.Value < 30) itemHealthLevel = HealthLevel.Warning;
                    _logger.Debug($"Aggregated License Info: Valid={licenseInfo.IsValid}, ExpiresInDays={licenseInfo.DaysUntilExpiry}. Item Health: {itemHealthLevel}");
                    break;
                case SystemErrorInfoSummaryDto errorInfo:
                    report.SystemErrorSummary = errorInfo;
                    if (errorInfo.CriticalErrorCountLast24Hours > 5) itemHealthLevel = HealthLevel.Error; // Example threshold
                    else if (errorInfo.CriticalErrorCountLast24Hours > 0) itemHealthLevel = HealthLevel.Warning;
                    _logger.Debug($"Aggregated System Error Info: CriticalErrors(24h)={errorInfo.CriticalErrorCountLast24Hours}. Item Health: {itemHealthLevel}");
                    break;
                case IEnumerable<AutomatedTaskStatusInfoDto> taskInfos:
                    report.AutomatedTaskStatuses = taskInfos.ToList();
                    if (taskInfos.Any(t => t.LastRunStatus == "Failed"))
                    {
                        itemHealthLevel = HealthLevel.Error;
                    }
                    else if (taskInfos.Any(t => t.LastRunStatus != "Success" && t.LastRunStatus != "Running" && t.NextRunTimestamp.HasValue && t.NextRunTimestamp.Value < DateTimeOffset.UtcNow))
                    {
                        // Task is overdue and not successful
                        itemHealthLevel = HealthLevel.Warning;
                    }
                    _logger.Debug($"Aggregated Automated Task Info: {taskInfos.Count()} tasks. Failed: {taskInfos.Count(t => t.LastRunStatus == "Failed")}. Item Health: {itemHealthLevel}");
                    break;
                default:
                    _logger.Warning($"Unknown data type received from source '{result.SourceType}': {data.GetType().FullName}");
                    report.AdditionalData.TryAdd($"UnknownData_{result.SourceType}", data);
                    break;
            }
            currentOverallHealthLevel = UpdateHealthLevel(currentOverallHealthLevel, itemHealthLevel);
        }

        report.OverallStatus = currentOverallHealthLevel.ToString();
        _logger.Info($"Health data aggregation completed. Overall system status: {report.OverallStatus}");

        // Optionally, update Prometheus metrics directly if configured
        _metricsCollector?.UpdateMetrics(report);

        return report;
    }

    private async Task<(string SourceType, object? DataObject, bool Success)> CollectDataFromSourceAsync(
        IHealthDataSource source, CancellationToken cancellationToken)
    {
        string sourceTypeName = source.GetType().Name.Replace("HealthDataSource", "").Replace("DataSource", "");
        _logger.Debug($"Attempting to collect data from source: {sourceTypeName}");
        try
        {
            var data = await source.GetHealthDataAsync(cancellationToken);
            _logger.Debug($"Successfully collected data from source: {sourceTypeName}");
            return (sourceTypeName, data, true);
        }
        catch (DataSourceUnavailableException ex)
        {
            _logger.Warning($"Data source {sourceTypeName} is unavailable. Message: {ex.Message}", ex);
            return (sourceTypeName, null, false); // Report as failure but don't stop aggregation
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"An unexpected error occurred while collecting data from source {sourceTypeName}.");
            return (sourceTypeName, null, false); // Report as failure but don't stop aggregation
        }
    }

    private HealthLevel UpdateHealthLevel(HealthLevel currentLevel, HealthLevel newItemLevel)
    {
        return (HealthLevel)Math.Max((int)currentLevel, (int)newItemLevel);
    }
}