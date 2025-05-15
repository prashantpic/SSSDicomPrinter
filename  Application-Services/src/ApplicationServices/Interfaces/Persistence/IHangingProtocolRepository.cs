using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TheSSS.DicomViewer.Application.Interfaces.Persistence
{
    public interface IHangingProtocolRepository
    {
        Task<HangingProtocol> GetByIdAsync(int protocolId, CancellationToken cancellationToken);
        Task<List<HangingProtocol>> GetByUserIdentifierAsync(string userIdentifier, CancellationToken cancellationToken);
        Task AddAsync(HangingProtocol protocol, CancellationToken cancellationToken);
        Task UpdateAsync(HangingProtocol protocol, CancellationToken cancellationToken);
        Task DeleteAsync(HangingProtocol protocol, CancellationToken cancellationToken);
    }
}