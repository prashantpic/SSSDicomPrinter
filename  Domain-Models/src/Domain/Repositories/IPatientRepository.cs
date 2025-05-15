using System.Threading.Tasks;
using TheSSS.DicomViewer.Domain.Core.Identifiers;
using TheSSS.DicomViewer.Domain.DicomAccess;

namespace TheSSS.DicomViewer.Domain.Repositories
{
    public interface IPatientRepository
    {
        Task<Patient?> GetByIdAsync(PatientId patientId);
        Task AddAsync(Patient patient);
    }
}