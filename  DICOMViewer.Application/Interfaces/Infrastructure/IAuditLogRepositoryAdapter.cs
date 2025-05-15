using System.Threading.Tasks;

namespace TheSSS.DICOMViewer.Application.Interfaces.Infrastructure;

public interface IAuditLogRepositoryAdapter
{
    Task LogEventAsync(string eventType, string eventDetails, string outcome, string userIdentifier = null, string sourceIp = null);
}