using System.Threading.Tasks;
using TheSSS.DicomViewer.Domain.Core.Identifiers;
using TheSSS.DicomViewer.Domain.Anonymization;

namespace TheSSS.DicomViewer.Domain.Repositories
{
    public interface IAnonymizationProfileRepository
    {
        Task<AnonymizationProfile?> GetByIdAsync(AnonymizationProfileId profileId);
    }
}