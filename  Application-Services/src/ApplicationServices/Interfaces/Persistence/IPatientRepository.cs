using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TheSSS.DicomViewer.Application.Interfaces.Persistence
{
    public interface IPatientRepository
    {
        Task<Domain.Models.Patient> GetByIdAsync(string patientId, CancellationToken cancellationToken = default);
        Task AddAsync(Domain.Models.Patient patient, CancellationToken cancellationToken = default);
        Task UpdateAsync(Domain.Models.Patient patient, CancellationToken cancellationToken = default);
        Task DeleteAsync(string patientId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Domain.Models.Patient>> GetAllAsync(CancellationToken cancellationToken = default);
    }
}