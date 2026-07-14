using Microsoft.EntityFrameworkCore;
using PromptlyNote.Core.Constants;
using PromptlyNote.Core.Entities;
using PromptlyNote.Core.Interfaces.Repositories;
using PromptlyNote.Core.Models;
using PromptlyNote.Core.Utils;
using System.Linq.Expressions;

namespace PromptlyNote.Data.Repositories
{
    public abstract class Repository<TEntity>(ApplicationDbContext context)
    : IDefaultRepository<TEntity>
    where TEntity : BaseEntity
    {
        protected readonly ApplicationDbContext Context = context;
        protected readonly DbSet<TEntity> DbSet = context.Set<TEntity>();

        public virtual async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            await DbSet.AddAsync(entity, cancellationToken);
        }

        public virtual async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            await DbSet.AddRangeAsync(entities, cancellationToken);
        }

        public virtual async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            await DbSet.Where(e => e.Id == id).ExecuteDeleteAsync(cancellationToken);
        }

        public virtual async Task<TEntity?> FindAsync(
            Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default,
            params Expression<Func<TEntity, object>>[] includes)
        {
            return await BuildQuery(predicate, null, includes)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public virtual async Task<TResult?> FindAsync<TResult>(
            Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, TResult>> selector,
            CancellationToken cancellationToken = default,
            params Expression<Func<TEntity, object>>[] includes)
        {
            return await BuildQuery(predicate, null, includes)
                .Select(selector)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public virtual async Task<bool> ExistsAsync(
            Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            return await DbSet.AnyAsync(predicate, cancellationToken);
        }

        public virtual async Task<PagedResult<TEntity>> ListAsync(
            Expression<Func<TEntity, bool>> predicate,
            int page = PaginationConfiguration.MinimumPage,
            int pageSize = PaginationConfiguration.DefaultPageSize,
            Expression<Func<TEntity, object>>? orderBy = null,
            CancellationToken cancellationToken = default,
            params Expression<Func<TEntity, object>>[] includes)
        {
            var query = BuildQuery(predicate, orderBy, includes);

            var count = await query.CountAsync(cancellationToken);

            (page, pageSize) = PaginationHelper.Normalize(page, pageSize);

            var items = await query
                .Skip(page * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<TEntity>(
                items,
                count,
                page,
                (int)Math.Ceiling(count / (double)pageSize));
        }

        public virtual async Task<PagedResult<TResult>> ListAsync<TResult>(
            Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, TResult>> selector,
            int page = PaginationConfiguration.MinimumPage,
            int pageSize = PaginationConfiguration.DefaultPageSize,
            Expression<Func<TEntity, object>>? orderBy = null,
            CancellationToken cancellationToken = default,
            params Expression<Func<TEntity, object>>[] includes)
        {
            var query = BuildQuery(predicate, orderBy, includes);

            var count = await query.CountAsync();

            (page, pageSize) = PaginationHelper.Normalize(page, pageSize);

            var items = await query
                .Skip(page * pageSize)
                .Take(pageSize)
                .Select(selector)
                .ToListAsync(cancellationToken);

            return new PagedResult<TResult>(
                items,
                count,
                page,
                (int)Math.Ceiling(count / (double)pageSize));
        }

        public virtual Task UpdateAsync(TEntity entity)
        {
            entity.UpdatedAt = DateTime.UtcNow;
            DbSet.Update(entity);
            return Task.CompletedTask;
        }

        public virtual Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            return DbSet.CountAsync(predicate, cancellationToken);
        }

        protected virtual IQueryable<TEntity> BuildQuery(
            Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>>? orderBy,
            params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = DbSet
                .AsNoTracking()
                .Where(predicate);

            if (includes.Length > 0)
            {
                query = query.AsSplitQuery();

                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            return orderBy is null
                ? query
                : query.OrderBy(orderBy);
        }
    }
}
