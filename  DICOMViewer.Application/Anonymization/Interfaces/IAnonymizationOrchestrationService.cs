using System.Collections.Generic;
using System.Threading.Tasks;
using TheSSS.DICOMViewer.Application.Anonymization.DTOs;

namespace TheSSS.DICOMViewer.Application.Anonymization.Interfaces
{
    public interface IAnonymizationOrchestrationService
    {
        Task<AnonymizationResultDto> AnonymizeDatasetAsync(AnonymizationRequestDto request);
        Task<AnonymizationProfileDto> CreateProfileAsync(AnonymizationProfileDto profileDto);
        Task<AnonymizationProfileDto> GetProfileByIdAsync(string profileId);
        Task<IEnumerable<AnonymizationProfileDto>> GetAllProfilesAsync();
        Task<AnonymizationProfileDto> UpdateProfileAsync(AnonymizationProfileDto profileDto);
        Task<bool> DeleteProfileAsync(string profileId);
    }
}