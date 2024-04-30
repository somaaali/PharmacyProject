﻿namespace Pharmacy.Repositories
{
    public interface IDataRepo<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync( int id );
        Task AddAsync( T entity );
        Task UpdateAsync( T entity );
        Task DeleteAsync( T entity );
        Task<bool> Save();
    }
}