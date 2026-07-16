using AutoMapper;
using PromptlyNote.Core.Constants;
using PromptlyNote.Core.DTOs;
using PromptlyNote.Core.DTOs.Forms.Create;
using PromptlyNote.Core.DTOs.Forms.Update;
using PromptlyNote.Core.Entities;
using PromptlyNote.Core.Enums;
using PromptlyNote.Core.Exceptions;
using PromptlyNote.Core.Interfaces;
using PromptlyNote.Core.Interfaces.Repositories;
using PromptlyNote.Core.Interfaces.Services;
using PromptlyNote.Core.Models;
using PromptlyNote.Core.Utils;
using System.Linq.Expressions;

namespace PromptlyNote.Services.Services
{
    public class CategoryService(
        ICategoryRepository categoryRepository,
        IMapper mapper,
        IUnitOfWork unitOfWork) : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository = categoryRepository;
        private readonly IMapper _mapper = mapper;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task CreateAsync(CreateCategoryForm form, string userId, CancellationToken cancellationToken = default)
        {
            var userGuid = userId.ParseToGuidWithThrow("user");

            if (await _categoryRepository.CountAsync(c => c.UserId == userGuid, cancellationToken) >= UserCollectionsMaximumCount.Categories)
                throw new LimitExceededException("You have reached the maximum number of categories allowed.");

            Category category = new(form.Name, form.ColorHex, userGuid);

            await _categoryRepository.AddAsync(category, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(string categoryId, string userId, CancellationToken cancellationToken = default)
        {
            var categoryGuid = categoryId.ParseToGuidWithThrow("category");
            var userGuid = userId.ParseToGuidWithThrow("user");

            var category = await _categoryRepository.FindAsync(
                predicate: c => c.Id == categoryGuid,
                selector: c => new { c.UserId, c.Default },
                cancellationToken: cancellationToken
            ) ?? throw new NotFoundException("category");

            if (category.UserId != userGuid)
                throw new ForbiddenException(ExceptionMessages.NoPermission("delete", "category"));

            if (category.Default)
                throw new ForbiddenException(ExceptionMessages.NoPermission("delete", "default category"));

            await _categoryRepository.DeleteAsync(categoryGuid, cancellationToken);
        }

        public async Task<CategoryDto> GetAsync(string categoryId, string userId, CancellationToken cancellationToken = default)
        {
            var categoryGuid = categoryId.ParseToGuidWithThrow("category");
            var userGuid = userId.ParseToGuidWithThrow("user");

            var category = await _categoryRepository.FindAsync(
                predicate: c => c.Id == categoryGuid,
                cancellationToken: cancellationToken
            ) ?? throw new NotFoundException("category");

            return category.UserId != userGuid
                ? throw new ForbiddenException(ExceptionMessages.NoPermission("access", "category"))
                : _mapper.Map<CategoryDto>(category);
        }

        public async Task<PagedResult<CategoryDto>> ListAsync(string userId, int page, int pageSize, CategorySortBy sortBy = CategorySortBy.Name, bool isDescending = false, CancellationToken cancellationToken = default)
        {
            var userGuid = userId.ParseToGuidWithThrow("user");

            PaginationHelper.ValidatePageSettings(page, pageSize);


            Expression<Func<Category, object>> orderBy = sortBy switch
            {
                CategorySortBy.Name => c => c.Name,
                CategorySortBy.Default => c => c.Default,
                _ => throw new ArgumentOutOfRangeException(nameof(sortBy), sortBy, "Invalid sort option")
            };

            var result = await _categoryRepository.ListAsync(
                predicate: c => c.UserId == userGuid,
                page: page,
                pageSize: pageSize,
                orderBy: orderBy,
                isDescending: isDescending,
                cancellationToken: cancellationToken
            );

            return new PagedResult<CategoryDto>(
                _mapper.Map<IReadOnlyCollection<CategoryDto>>(result.Data),
                result.Count,
                result.CurrentPage,
                result.TotalPages
            );
        }

        public async Task<PagedResult<CategoryDto>> SearchAsync(string term, string userId, int page = PaginationConfiguration.MinimumPage, int pageSize = PaginationConfiguration.DefaultPageSize, CancellationToken cancellationToken = default)
        {
            var userGuid = userId.ParseToGuidWithThrow("user");

            PaginationHelper.ValidatePageSettings(page, pageSize);

            var categories = await _categoryRepository.SearchAsync(userGuid, term, page, pageSize, cancellationToken);

            return new PagedResult<CategoryDto>(
                _mapper.Map<IReadOnlyCollection<CategoryDto>>(categories.Data),
                categories.Count,
                categories.CurrentPage,
                categories.TotalPages
            );
        }

        public async Task UpdateAsync(string categoryId, string userId, UpdateCategoryForm form, CancellationToken cancellationToken = default)
        {
            var categoryGuid = categoryId.ParseToGuidWithThrow("category");
            var userGuid = userId.ParseToGuidWithThrow("user");

            var category = await _categoryRepository.FindAsync(
                predicate: c => c.Id == categoryGuid,
                cancellationToken: cancellationToken
            ) ?? throw new NotFoundException("category");

            if (category.UserId != userGuid)
                throw new ForbiddenException(ExceptionMessages.NoPermission("update", "category"));

            if (category.Default)
                throw new ForbiddenException(ExceptionMessages.NoPermission("update", "default category"));

            category.Name = form.Name;
            category.ColorHex = form.ColorHex;
            category.UpdatedAt = DateTime.UtcNow;

            await _categoryRepository.UpdateAsync(category);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
