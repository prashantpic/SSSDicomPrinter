using System.Collections.Generic;
using System.Threading.Tasks;
using TheSSS.DICOMViewer.Application.Anonymization.DTOs;
using TheSSS.DICOMViewer.Application.DicomNetwork.DTOs;

namespace TheSSS.DICOMViewer.Application.Interfaces.Infrastructure;

public interface ISettingsRepositoryAdapter
{
    Task<AnonymizationProfileDto> GetAnonymizationProfileByIdAsync(string profileId);
    Task<IEnumerable<AnonymizationProfileDto>> GetAllAnonymizationProfilesAsync();
    Task<AnonymizationProfileDto> CreateAnonymizationProfileAsync(AnonymizationProfileDto profileDto);
    Task<AnonymizationProfileDto> UpdateAnonymizationProfileAsync(AnonymizationProfileDto profileDto);
    Task<bool> DeleteAnonymizationProfileAsync(string profileId);
    Task<PacsConfigurationDto> GetPacsConfigurationByIdAsync(string pacsNodeId);
    Task<IEnumerable<PacsConfigurationDto>> GetAllPacsConfigurationsAsync();
    Task<PacsConfigurationDto> CreatePacsConfigurationAsync(PacsConfigurationDto pacsConfigDto);
    Task<PacsConfigurationDto> UpdatePacsConfigurationAsync(PacsConfigurationDto pacsConfigDto);
    Task<bool> DeletePacsConfigurationAsync(string pacsNodeId);
    Task UpdatePacsConnectionStatusAsync(string pacsNodeId, System.DateTime lastTestTime, string lastTestResult);
    Task<PixelAnonymizationTemplateDto> GetPixelAnonymizationTemplateByIdAsync(int templateId);
}