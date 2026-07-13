using PromptlyNote.Core.Entities;
using PromptlyNote.Core.Interfaces.Repositories;

namespace PromptlyNote.Data.Repositories
{
    public class TaskListRepository(ApplicationDbContext context) : Repository<TaskList>(context), ITaskListRepository
    {
    }
}
