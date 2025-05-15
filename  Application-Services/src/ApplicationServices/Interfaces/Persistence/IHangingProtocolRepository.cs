using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TheSSS.DicomViewer.Application.Interfaces.Persistence
{
    public interface IHangingProtocolRepository
    {
        Task<Domain.Models.HangingProtocol> GetByIdAsync(int protocolId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Domain.Models.HangingProtocol>> GetByUserIdentifierAsync(string userIdentifier, CancellationToken cancellationToken = default);
        Task AddAsync(Domain.Models.HangingProtocol protocol, CancellationToken cancellationToken = default);
        Task UpdateAsync(Domain.Models.HangingProtocol protocol, CancellationToken cancellationToken = default);
        Task DeleteAsync(int protocolId, CancellationToken cancellationToken = default);
    }
}