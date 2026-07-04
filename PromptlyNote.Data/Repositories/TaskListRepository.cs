using Microsoft.EntityFrameworkCore;
using PromptlyNote.Core.Constants;
using PromptlyNote.Core.Entities;
using PromptlyNote.Core.Interfaces.Repositories;
using PromptlyNote.Core.Models;
using System.Linq.Expressions;

namespace PromptlyNote.Data.Repositories
{
    public class TaskListRepository(ApplicationDbContext context) : ITaskListRepository
    {
        private readonly ApplicationDbContext _context = context;

        public async Task AddAsync(TaskList taskList)
        {
            await _context.TaskLists.AddAsync(taskList);
            await _context.SaveChangesAsync();
        }

        public async Task AddRangeAsync(List<TaskList> taskLists)
        {
            await _context.TaskLists.AddRangeAsync(taskLists);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            await _context.TaskLists.Where(tl => tl.Id == id).ExecuteDeleteAsync();
        }

        public async Task<TaskList?> GetAsync(Guid id)
        {
            return await _context.TaskLists.FindAsync(id);
        }

        public async Task<bool> ExistsAsync(Expression<Func<TaskList, bool>> predicate)
        {
            return await _context.TaskLists.AsQueryable().AnyAsync(predicate);
        }

        public async Task<PagedResult<TaskList>> ListAsync(Expression<Func<TaskList, bool>> predicate, int page = 0, int pageSize = 10, Expression<Func<TaskList, object>>? orderBy = null, params Expression<Func<TaskList, object>>[] includes)
        {
            var query = _context.TaskLists.AsQueryable();
            query = query.Where(predicate);
            if (orderBy != null)
                query = query.OrderBy(orderBy);
            else
                query = query.OrderBy(u => u.Id);
            var count = await query.CountAsync();
            page = page < PaginationConfiguration.PageStart ? PaginationConfiguration.PageStart : page;
            pageSize = pageSize < 1 ? PaginationConfiguration.PageSize : pageSize;
            query = query.Skip(page * pageSize).Take(pageSize);
            var items = await query.ToListAsync();
            return new PagedResult<TaskList>(items, count, page, (int)Math.Ceiling(count / (double)pageSize));
        }

        public async Task UpdateAsync(TaskList taskList)
        {
            if (!taskList.Default)
                await _context.TaskLists.Where(tl => tl.Id == taskList.Id).ExecuteUpdateAsync(tl => tl
                    .SetProperty(tl => tl.Name, taskList.Name)
                    .SetProperty(tl => tl.Description, taskList.Description)
                    .SetProperty(tl => tl.IconName, taskList.IconName));
        }
    }
}
