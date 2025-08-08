// src/Core/GamingLibrary.Application/Common/Interfaces/IRepository.cs
// Purpose: Generic repository interface for data access
using System.Linq.Expressions;

namespace GamingLibrary.Application.Common.Interfaces
{
    /// <summary>
    /// Generic repository interface following Repository pattern
    /// </summary>
    public interface IRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(string id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(string id);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
        Task<long> CountAsync(Expression<Func<T, bool>>? predicate = null);
    }
}
