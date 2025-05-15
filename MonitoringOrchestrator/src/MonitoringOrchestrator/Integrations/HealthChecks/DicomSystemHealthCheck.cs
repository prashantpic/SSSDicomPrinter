using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using TheSSS.DICOMViewer.Monitoring.Contracts; // Assuming HealthReportDto and SystemHealthStatus enum are here
using TheSSS.DICOMViewer.Monitoring.UseCaseHandlers; // Assuming HealthAggregationService is here

namespace TheSSS.DICOMViewer.Monitoring.Integrations.HealthChecks
{
    /// <summary>
    /// Implements an ASP.NET Core <see cref="IHealthCheck"/> for the overall DICOM system.
    /// It queries the <see cref="HealthAggregationService"/> to determine the current health status.
    /// </summary>
    public class DicomSystemHealthCheck : IHealthCheck
    {
        private readonly HealthAggregationService _healthAggregationService; // Assuming this concrete type or an interface
        private readonly ILogger<DicomSystemHealthCheck> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DicomSystemHealthCheck"/> class.
        /// </summary>
        /// <param name="healthAggregationService">The service responsible for aggregating health data.</param>
        /// <param name="logger">The logger.</param>
        public DicomSystemHealthCheck(
            HealthAggregationService healthAggregationService, // To be replaced by IHealthAggregationService if defined
            ILogger<DicomSystemHealthCheck> logger)
        {
            _healthAggregationService = healthAggregationService ?? throw new ArgumentNullException(nameof(healthAggregationService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Runs the health check.
        /// </summary>
        /// <param name="context">A context object associated with the current health check.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the health check.</param>
        /// <returns>A <see cref="Task{HealthCheckResult}"/> that completes when the health check has finished,
        /// yielding the status of the component being checked.</returns>
        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Performing DicomSystemHealthCheck...");
            try
            {
                HealthReportDto healthReport = await _healthAggregationService.AggregateHealthAsync(cancellationToken);

                var healthStatus = healthReport.SystemStatus switch
                {
                    SystemHealthStatus.Healthy => Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Healthy,
                    SystemHealthStatus.Warning => Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Degraded,
                    SystemHealthStatus.Error => Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Unhealthy,
                    SystemHealthStatus.Unknown => Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Unhealthy, // Default to Unhealthy for Unknown
                    _ => Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Unhealthy,
                };

                var data = new Dictionary<string, object>
                {
                    { "Timestamp", healthReport.Timestamp },
                    { "OverallReportStatus", healthReport.SystemStatus.ToString() }
                };

                // Optionally, add more details from the health report to the 'data' dictionary
                // Example:
                // if (healthReport.StorageInfo != null) data.Add("StorageUsedPercentage", healthReport.StorageInfo.UsedPercentage);
                // if (healthReport.DatabaseConnectivity != null) data.Add("DatabaseConnected", healthReport.DatabaseConnectivity.IsConnected);
                // Add other relevant details from healthReport.Details or specific DTOs if needed by consumers of health check.
                // For simplicity, keeping it concise here.
                if (healthReport.Details != null)
                {
                    foreach(var detail in healthReport.Details)
                    {
                        data.TryAdd(detail.Key, detail.Value ?? "N/A");
                    }
                }


                string description = $"Overall system health status: {healthReport.SystemStatus}.";
                if (healthReport.SystemStatus != SystemHealthStatus.Healthy && !string.IsNullOrEmpty(healthReport.SystemErrorSummary?.ToString()))
                {
                    // Assuming SystemErrorSummaryDto has a meaningful ToString() or specific properties
                    description += $" Issues: {healthReport.SystemErrorSummary}";
                }


                _logger.LogInformation("DicomSystemHealthCheck completed with status: {Status}", healthStatus);
                return new HealthCheckResult(healthStatus, description, data: data);
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogWarning(ex, "DicomSystemHealthCheck was canceled.");
                return HealthCheckResult.Unhealthy("Health check was canceled.", ex, GetExceptionData(ex));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while performing DicomSystemHealthCheck.");
                return HealthCheckResult.Unhealthy("An unexpected error occurred during health check.", ex, GetExceptionData(ex));
            }
        }
        
        private static IReadOnlyDictionary<string, object> GetExceptionData(Exception ex)
        {
            return new Dictionary<string, object>
            {
                { "ExceptionType", ex.GetType().FullName ?? "Unknown" },
                { "ExceptionMessage", ex.Message },
                { "StackTrace", ex.StackTrace ?? "N/A" }
            };
        }
    }
}