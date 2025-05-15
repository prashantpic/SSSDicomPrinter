using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using TheSSS.DICOMViewer.Monitoring.Interfaces.Adapters;
using TheSSS.DICOMViewer.Monitoring.UseCaseHandlers;

namespace TheSSS.DICOMViewer.Monitoring.Integrations.HealthChecks;

/// <summary>
/// ASP.NET Core IHealthCheck implementation for overall system health.
/// </summary>
public class DicomSystemHealthCheck : IHealthCheck
{
    private readonly HealthAggregationService _healthAggregationService;
    private readonly ILoggerAdapter<DicomSystemHealthCheck> _logger;

    public DicomSystemHealthCheck(
        HealthAggregationService healthAggregationService,
        ILoggerAdapter<DicomSystemHealthCheck> logger)
    {
        _healthAggregationService = healthAggregationService;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        _logger.Debug("Performing ASP.NET Core health check via DicomSystemHealthCheck.");
        try
        {
            var healthReport = await _healthAggregationService.AggregateHealthDataAsync(cancellationToken);

            var status = healthReport.OverallStatus switch
            {
                "Healthy" => HealthStatus.Healthy,
                "Warning" => HealthStatus.Degraded,
                "Degraded" => HealthStatus.Degraded,
                "Error" => HealthStatus.Unhealthy,
                _ => HealthStatus.Unknown,
            };

            var description = $"Overall system health: {healthReport.OverallStatus}. Timestamp: {healthReport.Timestamp:yyyy-MM-dd HH:mm:ss 'UTC'}";

            // Prepare data for the health check result. Be mindful of exposing too much data.
            var data = new Dictionary<string, object?>
            {
                ["OverallStatus"] = healthReport.OverallStatus,
                ["Timestamp"] = healthReport.Timestamp,
                ["StorageUsedPercentage"] = healthReport.StorageInfo?.UsedPercentage,
                ["DatabaseConnected"] = healthReport.DatabaseConnectivity?.IsConnected,
                ["PACSConnections"] = healthReport.PacsConnections?.Select(p => new { p.PacsNodeId, p.IsConnected }).ToList(),
                ["LicenseValid"] = healthReport.LicenseStatus?.IsValid,
                ["LicenseDaysUntilExpiry"] = healthReport.LicenseStatus?.DaysUntilExpiry,
                ["CriticalErrorCountLast24Hours"] = healthReport.SystemErrorSummary?.CriticalErrorCountLast24Hours,
                ["AutomatedTasksFailedCount"] = healthReport.AutomatedTaskStatuses?.Count(t => t.LastRunStatus == "Failed")
            };

            if (status == HealthStatus.Unhealthy || status == HealthStatus.Degraded)
            {
                _logger.Warning($"DicomSystemHealthCheck returning {status}: {description}");
                // Optionally, add more specific error messages to the description or data
                if(healthReport.DatabaseConnectivity != null && !healthReport.DatabaseConnectivity.IsConnected)
                {
                    description += $" DB_Error: {healthReport.DatabaseConnectivity.ErrorMessage}.";
                }
                // Add more detailed error messages if OverallStatus is Error/Warning.
            }
            else
            {
                _logger.Info($"DicomSystemHealthCheck returning {status}: {description}");
            }

            return new HealthCheckResult(status, description, null, data);
        }
        catch (OperationCanceledException)
        {
            _logger.Warning("DicomSystemHealthCheck was canceled.");
            return HealthCheckResult.Unhealthy("Health check operation was canceled.", null);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "An unexpected error occurred during the DicomSystemHealthCheck.");
            return HealthCheckResult.Unhealthy("An internal error prevented the system health check.", ex, 
                new Dictionary<string, object?> { ["error"] = ex.Message });
        }
    }
}