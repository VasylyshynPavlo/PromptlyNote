using Microsoft.EntityFrameworkCore;
using PromptlyNote.Core.Constants;
using PromptlyNote.Core.Entities;
using PromptlyNote.Core.Interfaces.Repositories;
using PromptlyNote.Core.Models;
using PromptlyNote.Core.Utils;

namespace PromptlyNote.Data.Repositories
{
    public class ToDoTaskRepository(ApplicationDbContext context) : Repository<ToDoTask>(context), IToDoTaskRepository
    {
        protected readonly DbSet<ToDoTask> toDoTasks = context.Set<ToDoTask>();

        public async Task ReplaceSubTasksAsync(Guid taskId, List<SubTask> subTasks, CancellationToken cancellationToken = default)
        {
            var task = await toDoTasks
                .Include(t => t.SubTasks)
                .FirstOrDefaultAsync(t => t.Id == taskId, cancellationToken);

            if (task is null)
            {
                return;
            }

            task.SubTasks = subTasks;
            foreach (var subTask in subTasks)
            {
                Context.Entry(subTask).State = EntityState.Added;
            }
            task.UpdatedAt = DateTime.UtcNow;
        }

        public async Task<PagedResult<ToDoTask>> SearchAsync(Guid userId,
            string term,
            int page = PaginationConfiguration.MinimumPage,
            int pageSize = PaginationConfiguration.DefaultPageSize,
            CancellationToken cancellationToken = default)
        {
            var query = toDoTasks.AsQueryable().Where(
                (t => t.UserId == userId && (
                    t.Name.ToLower().Contains(term.ToLower()) ||
                    t.Note.ToLower().Contains(term.ToLower()) ||
                    t.SubTasks.Any(st => st.Name.ToLower().Contains(term.ToLower()))
                )));
            int count = await query.CountAsync();

            (page, pageSize) = PaginationHelper.Normalize(page, pageSize);

            var results = await query
                .Skip(page * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<ToDoTask>(results, count, page, (int)Math.Ceiling(count / (double)pageSize));
        }
    }
}
