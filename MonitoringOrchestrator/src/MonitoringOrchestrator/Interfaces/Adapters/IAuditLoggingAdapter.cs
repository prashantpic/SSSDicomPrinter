namespace TheSSS.DICOMViewer.Monitoring.Interfaces.Adapters;

public interface IAuditLoggingAdapter
{
    Task LogAuditEventAsync(string eventType, string eventDetails, string outcome, string sourceComponent);
}