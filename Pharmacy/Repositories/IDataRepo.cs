
namespace Pharmacy.Repositories
{
    public interface IDataRepo<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync( int id );
        Task<T> GetByNameAsync( string name );
        Task AddAsync( T entity );
        Task UpdateAsync( T entity );
        Task DeleteAsync( T entity );

        Task<bool> Save();

		Task<IEnumerable<T>> FilterMedicines( string keyword);
		Task<IEnumerable<T>> FilterMedicinesByCategory(string category);
		Task<IEnumerable<SearchHistory>> GetSearchHistoryByUserId(string userId); // Add this method

	}
}
