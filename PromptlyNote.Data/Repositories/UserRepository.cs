using Microsoft.EntityFrameworkCore;
using PromptlyNote.Core.Constants;
using PromptlyNote.Core.Entities;
using PromptlyNote.Core.Interfaces.Repositories;
using PromptlyNote.Core.Models;
using System.Linq.Expressions;

namespace PromptlyNote.Data.Repositories
{
    public class UserRepository(ApplicationDbContext context) : IUserRepository
    {
        private readonly ApplicationDbContext _context = context;

        public async Task AddAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task AddRangeAsync(List<User> users)
        {
            await _context.Users.AddRangeAsync(users);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            await _context.Users.Where(u => u.Id == id).ExecuteDeleteAsync();
        }

        public async Task<User?> GetAsync(Guid id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<bool> ExistsAsync(Expression<Func<User, bool>> predicate)
        {
            return await _context.Users.AnyAsync(predicate);
        }

        public async Task<PagedResult<User>> ListAsync(Expression<Func<User, bool>> predicate, int page = PaginationConfiguration.PageStart, int pageSize = PaginationConfiguration.PageSize, Expression<Func<User, object>>? orderBy = null, params Expression<Func<User, object>>[] includes)
        {
            var query = _context.Users.AsQueryable();
            query = query.Where(predicate);
            if (orderBy != null)
                query = query.OrderBy(orderBy);
            else
                query = query.OrderBy(u => u.Email);
            var count = await query.CountAsync();
            page = page < PaginationConfiguration.PageStart ? PaginationConfiguration.PageStart : page;
            pageSize = pageSize < 1 ? PaginationConfiguration.PageSize : pageSize;
            query = query.Skip(page * pageSize).Take(pageSize);
            var items = await query.ToListAsync();
            return new PagedResult<User>(items, count, page, (int)Math.Ceiling(count / (double)pageSize));
        }

        public async Task UpdateAsync(User user)
        {
            await _context.Users.Where(u => u.Id == user.Id).ExecuteUpdateAsync(u => u
                .SetProperty(u => u.FullName, user.FullName)
                .SetProperty(u => u.AvatarUrl, user.AvatarUrl));
        }
    }
}
