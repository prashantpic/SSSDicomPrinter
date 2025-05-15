using System.Threading.Tasks;

namespace TheSSS.DICOMViewer.Monitoring.Interfaces.Adapters;

/// <summary>
/// Adapter interface for interacting with the central AuditLogService (likely from REPO-CROSS-CUTTING).
/// </summary>
public interface IAuditLoggingAdapter
{
    /// <summary>
    /// Logs a monitoring-related audit event.
    /// </summary>
    /// <param name="eventType">The type of audit event (e.g., "AlertDispatched", "MonitoringWorkerStarted", "HealthCheckFailure").</param>
    /// <param name="eventDetails">Detailed description of the event. This can be a structured message or JSON string.</param>
    /// <param name="outcome">The outcome of the event (e.g., "Success", "Failure", "Skipped", "Throttled", "Deduplicated").</param>
    /// <param name="sourceComponent">The component originating the event (e.g., "SystemHealthMonitorWorker", "AlertDispatchService", "EmailAlertingChannel").</param>
    /// <param name="userId">Optional user ID if the event is related to a user action (typically null for system monitoring events).</param>
    /// <param name="correlationId">Optional correlation ID for tracking related events.</param>
    /// <returns>A Task representing the asynchronous logging operation.</returns>
    Task LogAuditEventAsync(
        string eventType,
        string eventDetails,
        string outcome,
        string sourceComponent,
        string? userId = null,
        string? correlationId = null);
}