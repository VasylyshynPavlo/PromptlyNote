using Microsoft.EntityFrameworkCore;
using PromptlyNote.Core.Constants;
using PromptlyNote.Core.Entities;
using PromptlyNote.Core.Interfaces.Repositories;
using PromptlyNote.Core.Models;
using PromptlyNote.Core.Utils;
using System.Linq.Expressions;

namespace PromptlyNote.Data.Repositories
{
    public class TaskListRepository(ApplicationDbContext context) : Repository<TaskList>(context), ITaskListRepository
    {
    }
}
