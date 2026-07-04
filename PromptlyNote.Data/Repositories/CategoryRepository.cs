using Microsoft.EntityFrameworkCore;
using PromptlyNote.Core.Constants;
using PromptlyNote.Core.Entities;
using PromptlyNote.Core.Interfaces.Repositories;
using PromptlyNote.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace PromptlyNote.Data.Repositories
{
    public class CategoryRepository(ApplicationDbContext context) : ICategoryRepository
    {
        private readonly ApplicationDbContext _context = context;

        public async Task AddAsync(Category category)
        {
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
        }

        public async Task AddRangeAsync(List<Category> categories)
        {
            await _context.Categories.AddRangeAsync(categories);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            await _context.Categories.Where(c => c.Id == id).ExecuteDeleteAsync();
        }

        public async Task<Category?> GetAsync(Guid id)
        {
            return await _context.Categories.FindAsync(id);
        }

        public async Task<bool> ExistsAsync(Expression<Func<Category, bool>> predicate)
        {
            return await _context.Categories.AsQueryable().AnyAsync(predicate);
        }

        public async Task<PagedResult<Category>> ListAsync(Expression<Func<Category, bool>> predicate, int page = 0, int pageSize = 10, Expression<Func<Category, object>>? orderBy = null, params Expression<Func<Category, object>>[] includes)
        {
            var query = _context.Categories.AsQueryable();
            query = query.Where(predicate);
            if (orderBy != null)
                query = query.OrderBy(orderBy);
            else
                query = query.OrderBy(u => u.Name);
            var count = await query.CountAsync();
            page = page < PaginationConfiguration.PageStart ? PaginationConfiguration.PageStart : page;
            pageSize = pageSize < 1 ? PaginationConfiguration.PageSize : pageSize;
            query = query.Skip(page * pageSize).Take(pageSize);
            var items = await query.ToListAsync();
            return new PagedResult<Category>(items, count, page, (int)Math.Ceiling(count / (double)pageSize));
        }

        public async Task UpdateAsync(Category category)
        {
            if (!category.Default)
                await _context.Categories.Where(c => c.Id == category.Id).ExecuteUpdateAsync(c => c
                    .SetProperty(c => c.Name, category.Name)
                    .SetProperty(c => c.ColorHex, category.ColorHex));
        }
    }
}
