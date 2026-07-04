using PromptlyNote.Core.Constants;
using PromptlyNote.Core.Entities;
using PromptlyNote.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace PromptlyNote.Core.Interfaces.Repositories
{
    public interface IDefaultRepository<T>
    {
        Task AddAsync(T item);
        Task AddRangeAsync(List<T> items);
        Task UpdateAsync(T item);
        Task<T?> GetAsync(Guid id);
        Task<PagedResult<T>> ListAsync(Expression<Func<T, bool>> predicate, int page = PaginationConfiguration.PageStart, int pageSize = PaginationConfiguration.PageSize, Expression<Func<T, object>>? orderBy = null, params Expression<Func<T, object>>[] includes);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
        Task DeleteAsync(Guid id);
    }
}
