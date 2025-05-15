namespace TheSSS.DICOMViewer.Monitoring.Interfaces.Adapters;

/// <summary>
/// Adapter interface for interacting with the central AuditLogService.
/// Implementation is expected to be provided by REPO-CROSS-CUTTING or a similar infrastructure layer.
/// </summary>
public interface IAuditLoggingAdapter
{
    /// <summary>
    /// Logs a monitoring-related audit event.
    /// </summary>
    /// <param name="eventType">The type of audit event (e.g., "SystemAlertDispatched", "MonitoringHealthCheckFailed").</param>
    /// <param name="eventDetails">Detailed information about the event (e.g., alert payload summary, error message).</param>
    /// <param name="outcome">The outcome of the event (e.g., "Success", "Failure", "Skipped").</param>
    /// <param name="sourceComponent">The component that initiated the event (e.g., "MonitoringOrchestrator.AlertDispatchService").</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task LogAuditEventAsync(string eventType, string eventDetails, string outcome, string sourceComponent);
}