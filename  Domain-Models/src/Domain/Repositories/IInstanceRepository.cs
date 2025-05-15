using System.Collections.Generic;
using System.Threading.Tasks;
using TheSSS.DicomViewer.Domain.Core.Identifiers;
using TheSSS.DicomViewer.Domain.DicomAccess;

namespace TheSSS.DicomViewer.Domain.Repositories
{
    public interface IInstanceRepository
    {
        Task<Instance?> GetByIdAsync(SOPInstanceUID instanceId);
        Task<IEnumerable<Instance>> GetBySeriesIdAsync(SeriesInstanceUID seriesId);
    }
}