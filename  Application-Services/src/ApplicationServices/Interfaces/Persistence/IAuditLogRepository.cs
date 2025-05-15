using System.Threading;
using System.Threading.Tasks;

namespace TheSSS.DicomViewer.Application.Interfaces.Persistence
{
    public interface IAuditLogRepository
    {
        Task AddAsync(AuditLog auditLog, CancellationToken cancellationToken);
        Task<List<AuditLog>> GetByUserIdentifierAsync(string userIdentifier, CancellationToken cancellationToken);
        Task<List<AuditLog>> GetByEventTypeAsync(string eventType, CancellationToken cancellationToken);
    }
}