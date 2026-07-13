using PromptlyNote.Core.Entities;
using PromptlyNote.Core.Interfaces.Repositories;

namespace PromptlyNote.Data.Repositories
{
    public class UserRepository(ApplicationDbContext context) : Repository<User>(context), IUserRepository
    {
    }
}
