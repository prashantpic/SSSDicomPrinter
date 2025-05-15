using TheSSS.DICOMViewer.Domain.Aggregates.AnonymizationProfileAggregate;

namespace TheSSS.DICOMViewer.Domain.Interfaces.Repositories;

public interface IAnonymizationProfileRepository
{
    Task<AnonymizationProfile?> GetByIdAsync(AnonymizationProfileId id);
    Task<AnonymizationProfile?> GetByNameAsync(string profileName);
    Task<IEnumerable<AnonymizationProfile>> GetAllAsync();
    Task AddAsync(AnonymizationProfile profile);
    Task UpdateAsync(AnonymizationProfile profile);
    Task DeleteAsync(AnonymizationProfileId id);
}