using System.Threading.Tasks;
using TheSSS.DicomViewer.Domain.Anonymization;
using TheSSS.DicomViewer.Domain.Core.Identifiers;

namespace TheSSS.DicomViewer.Domain.Repositories
{
    public interface IPixelAnonymizationTemplateRepository
    {
        Task<PixelAnonymizationTemplate?> GetByIdAsync(PixelAnonymizationTemplateId templateId);
    }
}