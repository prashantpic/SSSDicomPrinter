using Microsoft.Extensions.Diagnostics.HealthChecks;
using TheSSS.DICOMViewer.Monitoring.Contracts;
using TheSSS.DICOMViewer.Monitoring.UseCaseHandlers;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace TheSSS.DICOMViewer.Monitoring.Integrations.HealthChecks
{
    public class DicomSystemHealthCheck : IHealthCheck
    {
        private readonly HealthAggregationService _healthService;
        private readonly ILogger<DicomSystemHealthCheck> _logger;

        public DicomSystemHealthCheck(
            HealthAggregationService healthService,
            ILogger<DicomSystemHealthCheck> logger)
        {
            _healthService = healthService;
            _logger = logger;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var report = await _healthService.AggregateHealthDataAsync(cancellationToken);
                var status = MapHealthStatus(report.OverallStatus);
                
                var data = new Dictionary<string, object>
                {
                    ["Timestamp"] = report.Timestamp,
                    ["Status"] = report.OverallStatus.ToString()
                };

                if (report.StorageHealth != null) data.Add("Storage", report.StorageHealth);
                if (report.DatabaseHealth != null) data.Add("Database", report.DatabaseHealth);
                if (report.PacsConnections != null) data.Add("PACS", report.PacsConnections);

                return new HealthCheckResult(
                    status,
                    description: $"System health: {report.OverallStatus}",
                    data: data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Health check failure");
                return new HealthCheckResult(
                    HealthStatus.Unhealthy,
                    description: "Failed to collect system health data",
                    exception: ex);
            }
        }

        private HealthStatus MapHealthStatus(OverallHealthStatus status)
        {
            return status switch
            {
                OverallHealthStatus.Healthy => HealthStatus.Healthy,
                OverallHealthStatus.Warning => HealthStatus.Degraded,
                OverallHealthStatus.Error => HealthStatus.Unhealthy,
                _ => HealthStatus.Unhealthy
            };
        }
    }
}