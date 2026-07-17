using Microsoft.EntityFrameworkCore;
using PromptlyNote.Core.Constants;
using PromptlyNote.Core.Entities;
using PromptlyNote.Core.Utils;

namespace PromptlyNote.Data
{
    public class ApplicationDbSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            await context.Database.EnsureCreatedAsync();
        }
    }
}
