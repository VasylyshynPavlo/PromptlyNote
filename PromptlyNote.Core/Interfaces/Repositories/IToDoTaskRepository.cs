using PromptlyNote.Core.Entities;
using System.Linq.Expressions;

namespace PromptlyNote.Core.Interfaces.Repositories
{
    public interface IToDoTaskRepository : IDefaultRepository<ToDoTask>
    {
        Task AddSubTaskAsync(Guid taskId, SubTask subTask, CancellationToken cancellationToken = default);
        Task DeleteSubTaskAsync(Guid id, Guid taskId, CancellationToken cancellationToken = default);
        Task ReplaceSubTasksAsync(Guid taskId, List<SubTask> subTasks, CancellationToken cancellationToken = default);
        Task UpdateSubTaskAsync(Guid taskId, SubTask subTask, CancellationToken cancellationToken = default);
    }
}
