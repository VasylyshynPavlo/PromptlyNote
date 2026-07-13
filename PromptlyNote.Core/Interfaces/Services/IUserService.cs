using PromptlyNote.Core.DTOs;
using PromptlyNote.Core.DTOs.Forms.Create;
using PromptlyNote.Core.Enums;
using PromptlyNote.Core.Models;

namespace PromptlyNote.Core.Interfaces.Services
{
    public interface IUserService
    {
        Task CreateAsync(CreateUserForm form, bool viaGoogle, CancellationToken cancellationToken = default);
        Task DeleteAsync(string userId, CancellationToken cancellationToken = default);
        Task ChangeFullNameAsync(string fullName, string userId, CancellationToken cancellationToken = default);
        Task<PagedResult<UserDto>> ListAsync(string userId, int page = 0, int pageSize = 10, UserSortBy userSortBy = UserSortBy.FullName, bool includeTasks = false, bool includeCategory = false, bool includeTaskLists = false, CancellationToken cancellationToken = default);
        Task<UserDto?> GetAsync(string userId, bool includeTasks = false, bool includeCategory = false, bool includeTaskLists = false, CancellationToken cancellationToken = default);
        Task<UserDto?> GetByEmailAsync(string email, bool includeTasks = false, bool includeCategory = false, bool includeTaskLists = false, CancellationToken cancellationToken = default);
    }
}
