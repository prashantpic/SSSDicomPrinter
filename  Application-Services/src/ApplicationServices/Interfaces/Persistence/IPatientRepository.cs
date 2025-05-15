using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TheSSS.DicomViewer.Application.Interfaces.Persistence
{
    public interface IPatientRepository
    {
        Task<Patient> GetByIdAsync(string patientId, CancellationToken cancellationToken);
        Task AddAsync(Patient patient, CancellationToken cancellationToken);
        Task UpdateAsync(Patient patient, CancellationToken cancellationToken);
        Task DeleteAsync(Patient patient, CancellationToken cancellationToken);
        Task<bool> ExistsAsync(string patientId, CancellationToken cancellationToken);
    }
}