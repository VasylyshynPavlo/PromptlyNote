using PromptlyNote.Core.Constants;
using PromptlyNote.Core.DTOs;
using PromptlyNote.Core.DTOs.Forms.Create;
using PromptlyNote.Core.DTOs.Forms.Update;
using PromptlyNote.Core.Enums;
using PromptlyNote.Core.Models;

namespace PromptlyNote.Core.Interfaces.Services
{
    public interface ITaskListService
    {
        Task CreateAsync(CreateTaskListForm form, string userId, CancellationToken cancellationToken = default);
        Task DeleteAsync(string taskListId, string userId, CancellationToken cancellationToken = default);
        Task<PagedResult<TaskListDto>> ListAsync(string userId, int page = PaginationConfiguration.MinimumPage, int pageSize = PaginationConfiguration.DefaultPageSize, TaskListSortBy taskListSortBy = TaskListSortBy.Name, bool includeTasks = false, CancellationToken cancellationToken = default);
        Task<TaskListDto?> GetAsync(string taskListId, string userId, bool includeTasks = false, CancellationToken cancellationToken = default);
        Task UpdateAsync(string taskListId, string userId, UpdateTaskListForm form, CancellationToken cancellationToken = default);
    }
}
