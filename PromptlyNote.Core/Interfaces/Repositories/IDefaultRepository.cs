using PromptlyNote.Core.Constants;
using PromptlyNote.Core.Models;
using System.Linq.Expressions;

namespace PromptlyNote.Core.Interfaces.Repositories
{
    public interface IDefaultRepository<T>
    {
        Task AddAsync(T item, CancellationToken cancellationToken = default);
        Task AddRangeAsync(IEnumerable<T> items, CancellationToken cancellationToken = default);
        Task UpdateAsync(T item);
        Task<T?> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default, params Expression<Func<T, object>>[] includes);
        Task<T2?> FindAsync<T2>(Expression<Func<T, bool>> predicate, Expression<Func<T, T2>> selector, CancellationToken cancellationToken = default, params Expression<Func<T, object>>[] includes);
        Task<PagedResult<T>> ListAsync(Expression<Func<T, bool>> predicate, int page = PaginationConfiguration.MinimumPage, int pageSize = PaginationConfiguration.DefaultPageSize, Expression<Func<T, object>>? orderBy = null, CancellationToken cancellationToken = default, params Expression<Func<T, object>>[] includes);
        Task<PagedResult<T2>> ListAsync<T2>(Expression<Func<T, bool>> predicate, Expression<Func<T, T2>> selector, int page = PaginationConfiguration.MinimumPage, int pageSize = PaginationConfiguration.DefaultPageSize, Expression<Func<T, object>>? orderBy = null, CancellationToken cancellationToken = default, params Expression<Func<T, object>>[] includes);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<T, bool>> predicate,
            CancellationToken cancellationToken = default);
    }
}
