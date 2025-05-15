using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TheSSS.DicomViewer.Application.Interfaces.Persistence
{
    public interface IStudyRepository
    {
        Task<Domain.Models.Study> GetByStudyInstanceUidAsync(string studyInstanceUid, CancellationToken cancellationToken = default);
        Task AddAsync(Domain.Models.Study study, CancellationToken cancellationToken = default);
        Task UpdateAsync(Domain.Models.Study study, CancellationToken cancellationToken = default);
        Task DeleteAsync(string studyInstanceUid, CancellationToken cancellationToken = default);
        Task<IEnumerable<Domain.Models.Study>> GetByPatientIdAsync(string patientId, CancellationToken cancellationToken = default);
    }
}