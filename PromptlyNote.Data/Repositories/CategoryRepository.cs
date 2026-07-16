using Microsoft.EntityFrameworkCore;
using PromptlyNote.Core.Constants;
using PromptlyNote.Core.Entities;
using PromptlyNote.Core.Interfaces.Repositories;
using PromptlyNote.Core.Models;
using PromptlyNote.Core.Utils;

namespace PromptlyNote.Data.Repositories
{
    public class CategoryRepository(ApplicationDbContext context) : Repository<Category>(context), ICategoryRepository
    {
        private readonly DbSet<Category> _categories = context.Set<Category>();

        public async Task<PagedResult<Category>> SearchAsync(Guid userId,
            string term,
            int page = PaginationConfiguration.MinimumPage,
            int pageSize = PaginationConfiguration.DefaultPageSize,
            CancellationToken cancellationToken = default)
        {
            var query = _categories.AsQueryable().Where(c => c.UserId == userId && c.Name.ToLower().Contains(term.ToLower()));
            int count = await query.CountAsync();

            (page, pageSize) = PaginationHelper.Normalize(page, pageSize);

            var results = await query
                .Skip(page * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<Category>(results, count, page, (int)Math.Ceiling(count / (double)pageSize));
        }
    }
}
