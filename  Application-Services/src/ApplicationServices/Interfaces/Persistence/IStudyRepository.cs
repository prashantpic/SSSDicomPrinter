using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TheSSS.DicomViewer.Application.Interfaces.Persistence
{
    public interface IStudyRepository
    {
        Task<Study> GetByStudyInstanceUidAsync(string studyInstanceUid, CancellationToken cancellationToken);
        Task AddAsync(Study study, CancellationToken cancellationToken);
        Task UpdateAsync(Study study, CancellationToken cancellationToken);
        Task DeleteAsync(Study study, CancellationToken cancellationToken);
        Task<List<Study>> GetByPatientIdAsync(string patientId, CancellationToken cancellationToken);
    }
}