using TheSSS.DICOMViewer.Domain.Aggregates.PatientAggregate;

namespace TheSSS.DICOMViewer.Domain.Interfaces.Repositories;

public interface IPatientRepository
{
    Task<Patient?> GetByIdAsync(PatientId id);
    Task<Patient?> GetByPatientDicomIdAsync(string patientDicomId);
    Task AddAsync(Patient patient);
    Task UpdateAsync(Patient patient);
    Task DeleteAsync(PatientId id);
}