using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TheSSS.DicomViewer.Application.Interfaces.Persistence
{
    public interface IAnonymizationProfileRepository
    {
        Task<AnonymizationProfile> GetByIdAsync(int profileId, CancellationToken cancellationToken);
        Task<List<AnonymizationProfile>> GetAllAsync(CancellationToken cancellationToken);
        Task AddAsync(AnonymizationProfile profile, CancellationToken cancellationToken);
        Task UpdateAsync(AnonymizationProfile profile, CancellationToken cancellationToken);
        Task DeleteAsync(AnonymizationProfile profile, CancellationToken cancellationToken);
    }
}