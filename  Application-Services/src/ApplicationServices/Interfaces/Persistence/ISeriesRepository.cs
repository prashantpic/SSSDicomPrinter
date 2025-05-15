using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TheSSS.DicomViewer.Application.Interfaces.Persistence
{
    public interface ISeriesRepository
    {
        Task<Series> GetBySeriesInstanceUidAsync(string seriesInstanceUid, CancellationToken cancellationToken);
        Task AddAsync(Series series, CancellationToken cancellationToken);
        Task UpdateAsync(Series series, CancellationToken cancellationToken);
        Task DeleteAsync(Series series, CancellationToken cancellationToken);
        Task<List<Series>> GetByStudyInstanceUidAsync(string studyInstanceUid, CancellationToken cancellationToken);
    }
}