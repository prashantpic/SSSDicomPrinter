using Microsoft.EntityFrameworkCore;
using TheSSS.DICOMViewer.Infrastructure.Persistence.DbContext;
using TheSSS.DICOMViewer.Infrastructure.Persistence.Entities;

namespace TheSSS.DICOMViewer.Infrastructure.Persistence.Repositories
{
    public class PacsConfigurationRepository : IPacsConfigurationRepository
    {
        private readonly DicomDbContext _dbContext;

        public PacsConfigurationRepository(DicomDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PacsConfigurationDbo> GetByIdAsync(int id)
        {
            return await _dbContext.PacsConfigurations.FindAsync(id);
        }

        public async Task<IEnumerable<PacsConfigurationDbo>> GetAllAsync()
        {
            return await _dbContext.PacsConfigurations.ToListAsync();
        }

        public async Task AddAsync(PacsConfigurationDbo entity)
        {
            await _dbContext.PacsConfigurations.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(PacsConfigurationDbo entity)
        {
            _dbContext.PacsConfigurations.Update(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _dbContext.PacsConfigurations.Remove(entity);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}