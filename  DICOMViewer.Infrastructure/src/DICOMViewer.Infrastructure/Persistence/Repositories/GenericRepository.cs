using Microsoft.EntityFrameworkCore;
using TheSSS.DICOMViewer.Infrastructure.Persistence.DbContext;

namespace TheSSS.DICOMViewer.Infrastructure.Persistence.Repositories
{
    public class GenericRepository<TEntity> where TEntity : class
    {
        private readonly DicomDbContext _context;
        private readonly DbSet<TEntity> _dbSet;

        public GenericRepository(DicomDbContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        public async Task<TEntity?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task AddAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(TEntity entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(TEntity entity)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}