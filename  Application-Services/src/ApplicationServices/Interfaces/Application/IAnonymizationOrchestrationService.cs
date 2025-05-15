using System.Threading;
using System.Threading.Tasks;
using TheSSS.DicomViewer.Application.DTOs.Anonymization;

namespace TheSSS.DicomViewer.Application.Interfaces.Application
{
    public interface IAnonymizationOrchestrationService
    {
        Task<AnonymizationResultDto> ApplyProfileToDatasetAsync(string sopInstanceUid, int profileId, CancellationToken cancellationToken = default);
        Task<AnonymizationProfileDto> CreateAnonymizationProfileAsync(AnonymizationProfileConfigDto profileConfig, CancellationToken cancellationToken = default);
    }
}