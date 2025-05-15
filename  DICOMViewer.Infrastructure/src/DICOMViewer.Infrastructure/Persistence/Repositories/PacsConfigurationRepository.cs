using Microsoft.EntityFrameworkCore;
using TheSSS.DICOMViewer.Infrastructure.Persistence.DbContext;
using TheSSS.DICOMViewer.Infrastructure.Persistence.Entities;

namespace TheSSS.DICOMViewer.Infrastructure.Persistence.Repositories
{
    public class PacsConfigurationRepository
    {
        private readonly DicomDbContext _context;

        public PacsConfigurationRepository(DicomDbContext context)
        {
            _context = context;
        }

        public async Task<PacsConfigurationDbo?> GetByIdAsync(int id)
        {
            return await _context.PacsConfigurations.FindAsync(id);
        }

        public async Task<IEnumerable<PacsConfigurationDbo>> GetAllAsync()
        {
            return await _context.PacsConfigurations.ToListAsync();
        }

        public async Task AddAsync(PacsConfigurationDbo configuration)
        {
            await _context.PacsConfigurations.AddAsync(configuration);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(PacsConfigurationDbo configuration)
        {
            _context.PacsConfigurations.Update(configuration);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var config = await GetByIdAsync(id);
            if (config != null)
            {
                _context.PacsConfigurations.Remove(config);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<PacsConfigurationDbo?> GetByAeTitleAsync(string aeTitle)
        {
            return await _context.PacsConfigurations
                .FirstOrDefaultAsync(p => p.AeTitle == aeTitle);
        }
    }
}