using System.Threading.Tasks;

namespace TheSSS.DICOMViewer.Monitoring.Interfaces.Adapters;

/// <summary>
/// Adapter interface for interacting with a central AuditLogService.
/// This abstracts the actual audit logging implementation, typically provided by a cross-cutting concern or application service.
/// </summary>
public interface IAuditLoggingAdapter
{
    /// <summary>
    /// Logs a monitoring-related audit event.
    /// </summary>
    /// <param name="eventType">The type of event being logged (e.g., "AlertTriggered", "HealthCheckFailed").</param>
    /// <param name="eventDetails">Specific details about the event.</param>
    /// <param name="outcome">The outcome of the event (e.g., "Success", "Failure", "Throttled").</param>
    /// <param name="sourceComponent">The component within the monitoring system that generated the event.</param>
    /// <returns>A task that represents the asynchronous logging operation.</returns>
    Task LogAuditEventAsync(string eventType, string eventDetails, string outcome, string sourceComponent);
}