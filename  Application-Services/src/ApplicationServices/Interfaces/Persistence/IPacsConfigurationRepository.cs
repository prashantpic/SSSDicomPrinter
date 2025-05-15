using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TheSSS.DicomViewer.Application.Interfaces.Persistence
{
    public interface IPacsConfigurationRepository
    {
        Task<Domain.Models.PacsConfiguration> GetByIdAsync(int pacsNodeId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Domain.Models.PacsConfiguration>> GetAllAsync(CancellationToken cancellationToken = default);
        Task AddAsync(Domain.Models.PacsConfiguration pacsConfiguration, CancellationToken cancellationToken = default);
        Task UpdateAsync(Domain.Models.PacsConfiguration pacsConfiguration, CancellationToken cancellationToken = default);
        Task DeleteAsync(int pacsNodeId, CancellationToken cancellationToken = default);
    }
}