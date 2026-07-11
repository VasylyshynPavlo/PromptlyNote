using Microsoft.EntityFrameworkCore;
using PromptlyNote.Core.Constants;
using PromptlyNote.Core.Entities;
using PromptlyNote.Core.Interfaces.Repositories;
using PromptlyNote.Core.Models;
using PromptlyNote.Core.Utils;
using System.Linq.Expressions;

namespace PromptlyNote.Data.Repositories
{
    public class ToDoTaskRepository(ApplicationDbContext context) : Repository<ToDoTask>(context), IToDoTaskRepository
    {
        protected readonly DbSet<ToDoTask> toDoTasks = context.Set<ToDoTask>();

        public async Task AddSubTaskAsync(
            Guid taskId,
            SubTask subTask,
            CancellationToken cancellationToken = default)
        {
            var task = await toDoTasks
                .Include(t => t.SubTasks)
                .FirstOrDefaultAsync(t => t.Id == taskId, cancellationToken);

            if (task is null)
            {
                return;
            }

            task.SubTasks.Add(subTask);
            Context.Entry(subTask).State = EntityState.Added;
            task.UpdatedAt = DateTime.UtcNow;
        }

        public async Task DeleteSubTaskAsync(Guid id, Guid taskId, CancellationToken cancellationToken = default)
        {
            var task = await toDoTasks
                .Include(t => t.SubTasks)
                .FirstOrDefaultAsync(t => t.Id == taskId, cancellationToken);

            if (task is null)
            {
                return;
            }

            var subTask = task.SubTasks.FirstOrDefault(st => st.Id == id);

            if (subTask is null)
            {
                return;
            }

            task.SubTasks.Remove(subTask);
            Context.Entry(subTask).State = EntityState.Deleted;
            task.UpdatedAt = DateTime.UtcNow;
        }

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

        public async Task UpdateSubTaskAsync(Guid taskId, SubTask subTask, CancellationToken cancellationToken = default)
        {
            var task = await toDoTasks
                .Include(t => t.SubTasks)
                .FirstOrDefaultAsync(t => t.Id == taskId, cancellationToken);

            if (task is null)
            {
                return;
            }

            var existingSubTask = task.SubTasks.FirstOrDefault(st => st.Id == subTask.Id);

            if (existingSubTask is null)
            {
                return;
            }

            existingSubTask.Name = subTask.Name;
            existingSubTask.IsCompleted = subTask.IsCompleted;
            existingSubTask.UpdatedAt = DateTime.UtcNow;
            Context.Entry(existingSubTask).State = EntityState.Modified;
        }
    }
}
