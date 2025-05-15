namespace TheSSS.DICOMViewer.Monitoring.Integrations.HealthChecks;

using Microsoft.Extensions.Diagnostics.HealthChecks;
using TheSSS.DICOMViewer.Monitoring.UseCaseHandlers;
using TheSSS.DICOMViewer.Monitoring.Contracts;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class DicomSystemHealthCheck : IHealthCheck
{
    private readonly HealthAggregationService _healthAggregationService;
    private readonly ILogger<DicomSystemHealthCheck> _logger;

    public DicomSystemHealthCheck(
        HealthAggregationService healthAggregationService, // This should be resolved correctly based on DI (e.g. Scoped)
        ILogger<DicomSystemHealthCheck> logger)
    {
        _healthAggregationService = healthAggregationService;
        _logger = logger;
    }

    /// <summary>
    /// Runs the health check, aggregating data and translating it to ASP.NET Core HealthCheckResult.
    /// </summary>
    /// <param name="context">The context for the health check.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation, returning the HealthCheckResult.</returns>
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Running DicomSystemHealthCheck for ASP.NET Core Health Check endpoint.");

        try
        {
            // Aggregate health data
            var healthReport = await _healthAggregationService.AggregateHealthDataAsync(cancellationToken);

            // Translate overall status to HealthStatus enum
            var status = healthReport.OverallStatus switch
            {
                OverallHealthStatus.Healthy => HealthStatus.Healthy,
                OverallHealthStatus.Warning => HealthStatus.Degraded, // ASP.NET Core equivalent for Warning
                OverallHealthStatus.Error => HealthStatus.Unhealthy,
                OverallHealthStatus.Critical => HealthStatus.Unhealthy, // Critical also implies Unhealthy for this check
                _ => HealthStatus.Unknown // Default case, should ideally not happen with proper OverallStatus determination
            };

            // Prepare data for the health check result
            var data = new Dictionary<string, object>
            {
                { "Timestamp", healthReport.Timestamp },
                { "OverallStatus", healthReport.OverallStatus.ToString() }
            };

            // Add specific component health information for richer reporting
            if (healthReport.StorageHealth != null) data["StorageHealth"] = healthReport.StorageHealth;
            if (healthReport.DatabaseHealth != null) data["DatabaseHealth"] = healthReport.DatabaseHealth;
            if (healthReport.LicenseStatus != null) data["LicenseStatus"] = healthReport.LicenseStatus;
            if (healthReport.SystemErrorSummary != null) data["SystemErrorSummary"] = healthReport.SystemErrorSummary;
            if (healthReport.PacsConnections != null && healthReport.PacsConnections.Any()) data["PacsConnections"] = healthReport.PacsConnections;
            if (healthReport.AutomatedTaskStatuses != null && healthReport.AutomatedTaskStatuses.Any()) data["AutomatedTaskStatuses"] = healthReport.AutomatedTaskStatuses;


            string description = $"DICOM Viewer System Health: {healthReport.OverallStatus}.";
            if (status != HealthStatus.Healthy)
            {
                 var issues = new List<string>();
                 if (healthReport.DatabaseHealth?.IsConnected == false) issues.Add($"Database connection failed: {healthReport.DatabaseHealth.ErrorMessage ?? "No details"}.");
                 if (healthReport.LicenseStatus?.IsValid == false) issues.Add($"License invalid/expired: {healthReport.LicenseStatus.StatusMessage}.");
                 if (healthReport.PacsConnections != null)
                 {
                     var failedPacs = healthReport.PacsConnections.Where(p => !p.IsConnected).ToList();
                     if (failedPacs.Any()) issues.Add($"PACS connections failed for: {string.Join(", ", failedPacs.Select(p => p.PacsNodeId))}.");
                 }
                 if (healthReport.StorageHealth?.UsedPercentage > 95) issues.Add($"Storage usage critical: {healthReport.StorageHealth.UsedPercentage:F1}%."); // Example high threshold
                 else if (healthReport.StorageHealth?.UsedPercentage > 85) issues.Add($"Storage usage warning: {healthReport.StorageHealth.UsedPercentage:F1}%."); // Example warning threshold
                 if (healthReport.SystemErrorSummary?.CriticalErrorCountLast24Hours > 0) issues.Add($"Critical system errors detected: {healthReport.SystemErrorSummary.CriticalErrorCountLast24Hours} in last 24h.");
                 if (healthReport.AutomatedTaskStatuses != null)
                 {
                     var failedTasks = healthReport.AutomatedTaskStatuses.Where(t => t.LastRunStatus?.Equals("Failed", StringComparison.OrdinalIgnoreCase) == true).ToList();
                     if (failedTasks.Any()) issues.Add($"Failed automated tasks: {string.Join(", ", failedTasks.Select(t => t.TaskName))}.");
                 }

                 if (issues.Any())
                 {
                     description += $" Issues: {string.Join("; ", issues)}";
                 }
            }

            _logger.LogDebug($"DicomSystemHealthCheck result: {status}, Description: {description}");

            // Return the HealthCheckResult
            return new HealthCheckResult(
                status,
                description: description,
                data: data); // Include the detailed data
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning("DicomSystemHealthCheck was cancelled.");
            return new HealthCheckResult(HealthStatus.Unhealthy, "Health check was cancelled.", null,
                new Dictionary<string, object> { { "Cancellation", "Operation was cancelled by token." } });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while running the DicomSystemHealthCheck.");
            // If the monitoring system itself fails, report Unhealthy
            return new HealthCheckResult(
                HealthStatus.Unhealthy,
                "Monitoring system failed to collect health data.",
                exception: ex, // Include the exception details
                data: null);
        }
    }
}