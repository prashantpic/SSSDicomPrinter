using System.Collections.Generic;
using System.Threading.Tasks;
using TheSSS.DicomViewer.Domain.Core.Identifiers;
using TheSSS.DicomViewer.Domain.DicomAccess;

namespace TheSSS.DicomViewer.Domain.Repositories
{
    public interface IStudyRepository
    {
        Task<Study?> GetByIdAsync(StudyInstanceUID studyId);
        Task<IEnumerable<Study>> GetByPatientIdAsync(PatientId patientId);
    }
}