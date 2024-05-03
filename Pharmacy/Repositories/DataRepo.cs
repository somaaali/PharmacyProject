
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Expressions;

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

		#region Additional Operations

		// Update the DataRepo<T> class implementation
		public async Task<IEnumerable<T>> SearchMedicines(string keyword)
		{
			// Ensure that T is a type that has a Name property
			var nameProperty = typeof(T).GetProperty("Name");
			if (nameProperty == null)
			{
				throw new InvalidOperationException("Type T does not have a Name property.");
			}

			// Create an expression parameter for the entity
			var parameter = Expression.Parameter(typeof(T), "entity");
			// Create an expression to access the Name property
			var nameExpression = Expression.Property(parameter, nameProperty);
			// Create an expression to call Contains method on Name property
			var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
			var containsExpression = Expression.Call(nameExpression, containsMethod, Expression.Constant(keyword));
			// Create an expression to represent the lambda function
			var lambda = Expression.Lambda<Func<T, bool>>(containsExpression, parameter);

			// Apply the filter to the query
			return await _dbSet.Where(lambda).ToListAsync();
		}



		public async Task<IEnumerable<T>> FilterMedicinesByCategory(string category)
		{
			// Ensure T is Medicine or a type that has Category property
			if (typeof(T) != typeof(Medicine) && typeof(T).GetProperty("Category") == null)
			{
				throw new InvalidOperationException("Type T does not have a Category property.");
			}

			IQueryable<T> query = _dbSet;

			if (typeof(T) == typeof(Medicine))
			{
				query = query.Where(m => ((Medicine)(object)m).Category.Name == category);
			}
			else
			{
				// Get the Category property dynamically
				var categoryProperty = typeof(T).GetProperty("Category");
				// Create an expression parameter for the entity
				var parameter = Expression.Parameter(typeof(T), "entity");
				// Create an expression to access the Category property
				var categoryExpression = Expression.Property(parameter, categoryProperty);
				// Create an expression to access the Name property of Category
				var nameProperty = typeof(Category).GetProperty("Name");
				var nameExpression = Expression.Property(categoryExpression, nameProperty);
				// Create an expression to compare the Name property with the provided category
				var filterExpression = Expression.Equal(nameExpression, Expression.Constant(category));
				// Combine all expressions into a lambda expression
				var lambda = Expression.Lambda<Func<T, bool>>(filterExpression, parameter);
				// Apply the filter to the query
				query = query.Where(lambda);
			}

			return await query.ToListAsync();
		}



		public async Task<IEnumerable<SearchHistory>> GetSearchHistoryByUserId( string userId )
        {
            return await _dbSet.Cast<SearchHistory>().Where(s => s.UserId == userId).ToListAsync();
        }


        #endregion


    }
}
