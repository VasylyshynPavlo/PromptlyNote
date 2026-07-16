using PromptlyNote.Core.Constants;
using PromptlyNote.Core.Entities;
using PromptlyNote.Core.Models;

namespace PromptlyNote.Core.Interfaces.Repositories
{
    public interface ITaskListRepository : IDefaultRepository<TaskList>
    {
        Task<PagedResult<TaskList>> SearchAsync(Guid userId, string term, int page = PaginationConfiguration.MinimumPage, int pageSize = PaginationConfiguration.DefaultPageSize, CancellationToken cancellationToken = default);
    }
}
