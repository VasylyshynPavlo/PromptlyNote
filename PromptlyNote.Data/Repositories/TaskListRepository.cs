using Microsoft.EntityFrameworkCore;
using PromptlyNote.Core.Constants;
using PromptlyNote.Core.Entities;
using PromptlyNote.Core.Interfaces.Repositories;
using PromptlyNote.Core.Models;
using PromptlyNote.Core.Utils;

namespace PromptlyNote.Data.Repositories
{
    public class TaskListRepository(ApplicationDbContext context) : Repository<TaskList>(context), ITaskListRepository
    {
        private readonly DbSet<TaskList> _tasks = context.Set<TaskList>();

        public async Task<PagedResult<TaskList>> SearchAsync(Guid userId,
            string term,
            int page = PaginationConfiguration.MinimumPage,
            int pageSize = PaginationConfiguration.DefaultPageSize,
            CancellationToken cancellationToken = default)
        {
            var query = _tasks.AsQueryable().Where(t => t.UserId == userId && t.Name.ToLower().Contains(term.ToLower()));
            int count = await query.CountAsync();

            (page, pageSize) = PaginationHelper.Normalize(page, pageSize);

            var results = await query
                .Skip(page * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<TaskList>(results, count, page, (int)Math.Ceiling(count / (double)pageSize));
        }
    }
}
