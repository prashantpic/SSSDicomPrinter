using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TheSSS.DicomViewer.Application.Interfaces.Persistence
{
    public interface IPixelAnonymizationTemplateRepository
    {
        Task<Domain.Models.PixelAnonymizationTemplate> GetByIdAsync(int templateId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Domain.Models.PixelAnonymizationTemplate>> GetAllAsync(CancellationToken cancellationToken = default);
        Task AddAsync(Domain.Models.PixelAnonymizationTemplate template, CancellationToken cancellationToken = default);
        Task UpdateAsync(Domain.Models.PixelAnonymizationTemplate template, CancellationToken cancellationToken = default);
        Task DeleteAsync(int templateId, CancellationToken cancellationToken = default);
    }
}