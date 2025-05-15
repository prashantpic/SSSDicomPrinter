using System.Threading.Tasks;
using TheSSS.DICOMViewer.Application.Common.DTOs;

namespace TheSSS.DICOMViewer.Application.Interfaces.Infrastructure;

public interface IMetadataRepositoryAdapter
{
    Task AddPatientAsync(PatientDto patient);
    Task AddStudyAsync(StudyDto study);
    Task AddSeriesAsync(SeriesDto series);
    Task AddInstanceAsync(InstanceDto instance);
    Task<InstanceDto> GetInstanceAsync(string sopInstanceUid);
    Task UpdateInstanceAnonymizationStatusAsync(string sopInstanceUid, bool isAnonymized, string anonymizationProfileId);
}