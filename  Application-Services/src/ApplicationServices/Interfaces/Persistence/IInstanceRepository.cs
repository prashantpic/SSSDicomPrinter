using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TheSSS.DicomViewer.Application.Interfaces.Persistence
{
    public interface IInstanceRepository
    {
        Task<Domain.Models.Instance> GetBySopInstanceUidAsync(string sopInstanceUid, CancellationToken cancellationToken = default);
        Task AddAsync(Domain.Models.Instance instance, CancellationToken cancellationToken = default);
        Task UpdateAsync(Domain.Models.Instance instance, CancellationToken cancellationToken = default);
        Task DeleteAsync(string sopInstanceUid, CancellationToken cancellationToken = default);
        Task<IEnumerable<Domain.Models.Instance>> GetBySeriesInstanceUidAsync(string seriesInstanceUid, CancellationToken cancellationToken = default);
    }
}