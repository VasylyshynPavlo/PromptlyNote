using PromptlyNote.Core.Entities;

namespace PromptlyNote.Core.Interfaces.Services
{
    public interface IJwtService
    {
        string GenerateAccessToken(User user);
    }
}
