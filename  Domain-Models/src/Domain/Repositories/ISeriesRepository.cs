using System.Collections.Generic;
using System.Threading.Tasks;
using TheSSS.DicomViewer.Domain.Core.Identifiers;
using TheSSS.DicomViewer.Domain.DicomAccess;

namespace TheSSS.DicomViewer.Domain.Repositories
{
    public interface ISeriesRepository
    {
        Task<Series?> GetByIdAsync(SeriesInstanceUID seriesId);
        Task<IEnumerable<Series>> GetByStudyIdAsync(StudyInstanceUID studyId);
    }
}