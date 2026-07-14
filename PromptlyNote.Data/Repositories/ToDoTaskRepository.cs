using Microsoft.EntityFrameworkCore;
using PromptlyNote.Core.Entities;
using PromptlyNote.Core.Interfaces.Repositories;

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
    }
}
