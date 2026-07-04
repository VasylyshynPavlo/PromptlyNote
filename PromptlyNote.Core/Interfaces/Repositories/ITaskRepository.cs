using PromptlyNote.Core.Entities;
using System.Linq.Expressions;

namespace PromptlyNote.Core.Interfaces.Repositories
{
    public interface ITaskRepository : IDefaultRepository<ToDoTask>
    {
        Task AddSubTaskAsync(Guid taskId, SubTask subTask);
        Task DeleteSubTaskAsync(int id);
        Task ReplaceSubTasksAsync(Guid taskId, List<SubTask> subTasks);
        Task UpdateSubTaskAsync(Guid taskId, SubTask subTask);
    }
}
