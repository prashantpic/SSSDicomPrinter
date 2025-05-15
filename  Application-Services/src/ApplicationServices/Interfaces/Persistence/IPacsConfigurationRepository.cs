using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TheSSS.DicomViewer.Application.Interfaces.Persistence
{
    public interface IPacsConfigurationRepository
    {
        Task<PacsConfiguration> GetByIdAsync(int pacsNodeId, CancellationToken cancellationToken);
        Task<List<PacsConfiguration>> GetAllAsync(CancellationToken cancellationToken);
        Task AddAsync(PacsConfiguration pacsConfiguration, CancellationToken cancellationToken);
        Task UpdateAsync(PacsConfiguration pacsConfiguration, CancellationToken cancellationToken);
        Task UpdateStatusAsync(int pacsNodeId, string status, CancellationToken cancellationToken);
    }
}