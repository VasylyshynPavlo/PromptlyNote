using PromptlyNote.Core.Entities;
using PromptlyNote.Core.Interfaces.Repositories;

namespace PromptlyNote.Data.Repositories
{
    public class CategoryRepository(ApplicationDbContext context) : Repository<Category>(context), ICategoryRepository
    {
    }
}
