using PromptlyNote.Core.Constants;
using PromptlyNote.Core.Entities;
using PromptlyNote.Core.Models;

namespace PromptlyNote.Core.Interfaces.Repositories
{
    public interface IToDoTaskRepository : IDefaultRepository<ToDoTask>
    {
        Task ReplaceSubTasksAsync(Guid taskId, List<SubTask> subTasks, CancellationToken cancellationToken = default);
        Task<PagedResult<ToDoTask>> SearchAsync(Guid userId, string term, int page = PaginationConfiguration.MinimumPage, int pageSize = PaginationConfiguration.DefaultPageSize, CancellationToken cancellationToken = default);
    }
}
