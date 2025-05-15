using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TheSSS.DicomViewer.Application.Interfaces.Persistence
{
    public interface ISeriesRepository
    {
        Task<Domain.Models.Series> GetBySeriesInstanceUidAsync(string seriesInstanceUid, CancellationToken cancellationToken = default);
        Task AddAsync(Domain.Models.Series series, CancellationToken cancellationToken = default);
        Task UpdateAsync(Domain.Models.Series series, CancellationToken cancellationToken = default);
        Task DeleteAsync(string seriesInstanceUid, CancellationToken cancellationToken = default);
        Task<IEnumerable<Domain.Models.Series>> GetByStudyInstanceUidAsync(string studyInstanceUid, CancellationToken cancellationToken = default);
    }
}