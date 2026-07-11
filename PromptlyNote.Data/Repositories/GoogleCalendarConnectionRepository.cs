using PromptlyNote.Core.Entities;
using PromptlyNote.Core.Interfaces.Repositories;

namespace PromptlyNote.Data.Repositories
{
    public class GoogleCalendarConnectionRepository(ApplicationDbContext context)
        : Repository<GoogleCalendarConnection>(context), IGoogleCalendarConnectionRepository
    {
    }
}
