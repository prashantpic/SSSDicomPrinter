using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TheSSS.DicomViewer.Application.Interfaces.Persistence
{
    public interface IAuditLogRepository
    {
        Task AddAsync(Domain.Models.AuditLog auditLog, CancellationToken cancellationToken = default);
        Task<IEnumerable<Domain.Models.AuditLog>> GetByEventTypeAsync(string eventType, CancellationToken cancellationToken = default);
        Task<IEnumerable<Domain.Models.AuditLog>> GetByUserIdentifierAsync(string userIdentifier, CancellationToken cancellationToken = default);
    }
}