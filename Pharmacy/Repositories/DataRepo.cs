
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Pharmacy.Repositories
{
    public class DataRepo<T> : IDataRepo<T> where T : class
    {
        private readonly PharmacyDbContext _context;
        private readonly DbSet<T> _dbSet;
        public DataRepo( PharmacyDbContext context )
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        #region Crud
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T> GetByIdAsync( int id )
        {
            return await _dbSet.FindAsync(id);
        }   
        public async Task<T> GetByNameAsync( string name )
        {
            return await _dbSet.FirstOrDefaultAsync(x => EF.Property<string>(x, "Name") == name);
        }

        public async Task AddAsync( T entity )
        {
            await _dbSet.AddAsync(entity);
        }

        public async Task UpdateAsync( T entity )
        {
            _context.Entry(entity).State = EntityState.Modified;  // mmkn feh moshkla hena
        }

        public async Task DeleteAsync( T entity )
        {
            _dbSet.Remove(entity);
        }

        public async Task<bool> Save()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        #endregion
    }
}
