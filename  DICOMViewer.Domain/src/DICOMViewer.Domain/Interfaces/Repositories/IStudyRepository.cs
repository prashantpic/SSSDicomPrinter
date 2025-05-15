using TheSSS.DICOMViewer.Domain.Aggregates.StudyAggregate;
using TheSSS.DICOMViewer.Domain.Aggregates.PatientAggregate;

namespace TheSSS.DICOMViewer.Domain.Interfaces.Repositories;

public interface IStudyRepository
{
    Task<Study?> GetByIdAsync(StudyInstanceUid id);
    Task AddAsync(Study study);
    Task UpdateAsync(Study study);
    Task<IEnumerable<Study>> FindByPatientIdAsync(PatientId patientId);
}