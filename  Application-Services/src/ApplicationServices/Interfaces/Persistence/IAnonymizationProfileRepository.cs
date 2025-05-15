using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TheSSS.DicomViewer.Application.Interfaces.Persistence
{
    public interface IAnonymizationProfileRepository
    {
        Task<Domain.Models.AnonymizationProfile> GetByIdAsync(int profileId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Domain.Models.AnonymizationProfile>> GetAllAsync(CancellationToken cancellationToken = default);
        Task AddAsync(Domain.Models.AnonymizationProfile profile, CancellationToken cancellationToken = default);
        Task UpdateAsync(Domain.Models.AnonymizationProfile profile, CancellationToken cancellationToken = default);
        Task DeleteAsync(int profileId, CancellationToken cancellationToken = default);
    }
}