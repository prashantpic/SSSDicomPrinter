using System.Threading;
using System.Threading.Tasks;

namespace TheSSS.DicomViewer.Application.Interfaces.Application
{
    public interface IAnonymizationOrchestrationService
    {
        Task<AnonymizationResultDto> ApplyProfileToDatasetAsync(string sopInstanceUid, int profileId, CancellationToken cancellationToken);
        Task<AnonymizationProfileDto> CreateAnonymizationProfileAsync(AnonymizationProfileConfigDto profileConfig, CancellationToken cancellationToken);
    }
}