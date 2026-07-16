using PromptlyNote.Core.Constants;
using PromptlyNote.Core.DTOs;
using PromptlyNote.Core.DTOs.Forms.Create;
using PromptlyNote.Core.DTOs.Forms.Update;
using PromptlyNote.Core.Enums;
using PromptlyNote.Core.Models;

namespace PromptlyNote.Core.Interfaces.Services
{
    public interface ICategoryService
    {
        Task CreateAsync(CreateCategoryForm form, string userId, CancellationToken cancellationToken = default);
        Task UpdateAsync(string id, string userId, UpdateCategoryForm form, CancellationToken cancellationToken = default);
        Task DeleteAsync(string id, string userId, CancellationToken cancellationToken = default);
        Task<PagedResult<CategoryDto>> ListAsync(string userId, int page = PaginationConfiguration.MinimumPage, int pageSize = PaginationConfiguration.DefaultPageSize, CategorySortBy sortBy = CategorySortBy.Name, bool isDescending = false, CancellationToken cancellationToken = default);
        Task<CategoryDto> GetAsync(string id, string userId, CancellationToken cancellationToken = default);
        Task<PagedResult<CategoryDto>> SearchAsync(string term, string userId, int page = PaginationConfiguration.MinimumPage, int pageSize = PaginationConfiguration.DefaultPageSize, CancellationToken cancellationToken = default);
    }
}
