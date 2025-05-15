using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TheSSS.DicomViewer.Application.Interfaces.Persistence
{
    public interface IPixelAnonymizationTemplateRepository
    {
        Task<PixelAnonymizationTemplate> GetByIdAsync(int templateId, CancellationToken cancellationToken);
        Task<List<PixelAnonymizationTemplate>> GetAllAsync(CancellationToken cancellationToken);
        Task AddAsync(PixelAnonymizationTemplate template, CancellationToken cancellationToken);
        Task UpdateAsync(PixelAnonymizationTemplate template, CancellationToken cancellationToken);
        Task DeleteAsync(PixelAnonymizationTemplate template, CancellationToken cancellationToken);
    }
}