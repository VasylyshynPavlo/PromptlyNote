using PromptlyNote.Core.Entities;

namespace PromptlyNote.Core.Interfaces.Repositories
{
    public interface IToDoTaskRepository : IDefaultRepository<ToDoTask>
    {
        Task ReplaceSubTasksAsync(Guid taskId, List<SubTask> subTasks, CancellationToken cancellationToken = default);
    }
}
