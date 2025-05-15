namespace TheSSS.DICOMViewer.Monitoring.Interfaces.Adapters;

using System.Threading.Tasks;

public interface IAuditLoggingAdapter
{
    Task LogAuditEventAsync(string eventType, string eventDetails, string outcome, string sourceComponent);
}