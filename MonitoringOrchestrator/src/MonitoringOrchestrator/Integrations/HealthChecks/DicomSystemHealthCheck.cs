using Microsoft.Extensions.Diagnostics.HealthChecks;
using TheSSS.DICOMViewer.Monitoring.Contracts;
using TheSSS.DICOMViewer.Monitoring.UseCaseHandlers;
using System.Threading;
using System.Threading.Tasks;

namespace TheSSS.DICOMViewer.Monitoring.Integrations.HealthChecks;

public class DicomSystemHealthCheck : IHealthCheck
{
    private readonly HealthAggregationService _aggregationService;

    public DicomSystemHealthCheck(HealthAggregationService aggregationService)
    {
        _aggregationService = aggregationService;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var report = await _aggregationService.AggregateHealthDataAsync(cancellationToken);
            
            var status = report.OverallStatus switch
            {
                OverallHealthStatus.Healthy => HealthStatus.Healthy,
                OverallHealthStatus.Warning => HealthStatus.Degraded,
                OverallHealthStatus.Error => HealthStatus.Unhealthy,
                _ => HealthStatus.Unhealthy
            };

            return new HealthCheckResult(
                status,
                description: $"System health: {report.OverallStatus}",
                data: new()
                {
                    ["Timestamp"] = report.Timestamp,
                    ["StorageUsed"] = report.StorageHealth?.UsedPercentage,
                    ["DatabaseConnected"] = report.DatabaseHealth?.IsConnected
                });
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(
                description: "Failed to collect system health data",
                exception: ex);
        }
    }
}