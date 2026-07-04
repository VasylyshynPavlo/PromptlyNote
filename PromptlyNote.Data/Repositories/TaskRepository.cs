using Microsoft.EntityFrameworkCore;
using PromptlyNote.Core.Constants;
using PromptlyNote.Core.Entities;
using PromptlyNote.Core.Interfaces.Repositories;
using PromptlyNote.Core.Models;
using System.Linq.Expressions;

namespace PromptlyNote.Data.Repositories
{
    public class TaskRepository(ApplicationDbContext context) : ITaskRepository
    {
        private readonly ApplicationDbContext _context = context;

        public async Task AddAsync(ToDoTask task)
        {
            await _context.Tasks.AddAsync(task);
            await _context.SaveChangesAsync();
        }

        public async Task AddRangeAsync(List<ToDoTask> tasks)
        {
            await _context.Tasks.AddRangeAsync(tasks);
            await _context.SaveChangesAsync();
        }

        public async Task AddSubTaskAsync(Guid taskId, SubTask subTask)
        {
            var task = await _context.Tasks.Include(t => t.SubTasks).Where(t => t.Id == taskId).FirstOrDefaultAsync();
            if (task == null) return;
            task.SubTasks.Add(subTask);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            await _context.Tasks.Where(t => t.Id == id).ExecuteDeleteAsync();
        }

        public async Task DeleteSubTaskAsync(int id)
        {
            var task = await _context.Tasks.Include(t => t.SubTasks).Where(t => t.SubTasks.Any(st => st.Id == id)).FirstOrDefaultAsync();
            if (task == null) return;
            var subTask = task.SubTasks.FirstOrDefault(st => st.Id == id);
            if (subTask == null) return;
            task.SubTasks.Remove(subTask);
            await _context.SaveChangesAsync();
        }

        public async Task<ToDoTask?> GetAsync(Guid id)
        {
            return await _context.Tasks.FindAsync(id);
        }

        public async Task<bool> ExistsAsync(Expression<Func<ToDoTask, bool>> predicate)
        {
            return await _context.Tasks.AnyAsync(predicate);
        }

        public async Task<PagedResult<ToDoTask>> ListAsync(Expression<Func<ToDoTask, bool>> predicate, int page = 0, int pageSize = 10, Expression<Func<ToDoTask, object>>? orderBy = null, params Expression<Func<ToDoTask, object>>[] includes)
        {
            var query = _context.Tasks.AsQueryable();
            query = query.Where(predicate);
            if (orderBy != null)
                query = query.OrderBy(orderBy);
            else
                query = query.OrderBy(t => t.CreatedAt);
            var count = await query.CountAsync();
            page = page < PaginationConfiguration.PageStart ? PaginationConfiguration.PageStart : page;
            pageSize = pageSize < 1 ? PaginationConfiguration.PageSize : pageSize;
            query = query.Skip(page * pageSize).Take(pageSize);
            var items = await query.ToListAsync();
            return new PagedResult<ToDoTask>(items, count, page, (int)Math.Ceiling(count / (double)pageSize));
        }

        public async Task ReplaceSubTasksAsync(Guid taskId, List<SubTask> subTasks)
        {
            var task = await _context.Tasks.Include(t => t.SubTasks).Where(t => t.Id == taskId).FirstOrDefaultAsync();
            if (task == null) return;
            task.SubTasks = subTasks;
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ToDoTask task)
        {
            await _context.Tasks.Where(t => t.Id == task.Id).ExecuteUpdateAsync(t => t
                .SetProperty(p => p.Name, task.Name)
                .SetProperty(p => p.Note, task.Note)
                .SetProperty(p => p.DueDate, task.DueDate)
                .SetProperty(p => p.IsCompleted, task.IsCompleted)
                .SetProperty(p => p.CategoryId, task.CategoryId)
                .SetProperty(p => p.TaskListId, task.TaskListId));
        }

        public async Task UpdateSubTaskAsync(Guid taskId, SubTask subTask)
        {
            var task = await _context.Tasks.Include(t => t.SubTasks).Where(t => t.Id == taskId).FirstOrDefaultAsync();
            if (task == null) return;
        }
    }
}
