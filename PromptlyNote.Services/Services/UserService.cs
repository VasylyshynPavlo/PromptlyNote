using AutoMapper;
using PromptlyNote.Core.Constants;
using PromptlyNote.Core.DTOs;
using PromptlyNote.Core.DTOs.Forms.Create;
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
    public class UserService(
        IUserRepository userRepository,
        ITaskListRepository taskListRepository,
        ICategoryRepository categoryRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper) : IUserService
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ICategoryRepository _categoryRepository = categoryRepository;
        private readonly ITaskListRepository _taskListRepository = taskListRepository;
        private readonly IMapper _mapper = mapper;

        public async Task ChangeFullNameAsync(string fullName, string userId, CancellationToken cancellationToken = default)
        {
            var userGuid = userId.ParseToGuidWithThrow("user");

            var user = await _userRepository.FindAsync(
                predicate: u => u.Id == userGuid,
                cancellationToken: cancellationToken
            ) ?? throw new NotFoundException("user");

            user.FullName = fullName;

            await _userRepository.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task CreateAsync(CreateUserForm form, bool viaGoogle, CancellationToken cancellationToken = default)
        {
            if (await _userRepository.ExistsAsync(u => u.Email == form.Email, cancellationToken))
                throw new ConflictException(ExceptionMessages.ConflictFieldsName("user", "email"));

            if (form.Password is not null && viaGoogle)
                throw new ArgumentException("Password should not be provided when registering via Google.", nameof(form));

            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                User newUser = new()
                {
                    FullName = form.FullName,
                    PasswordHash = form.Password is not null ? BCrypt.Net.BCrypt.HashPassword(form.Password) : null,
                    GoogleAuth = viaGoogle,
                    Email = form.Email
                };
                await _userRepository.AddAsync(newUser, cancellationToken);

                List<TaskList> defaultTaskLists = [.. NewUserConfiguration.DefaultList
                    .Select(taskList => new TaskList
                    {
                        Name = taskList.Name,
                        IconName = taskList.IconName,
                        Description = taskList.Description,
                        Default = true,
                        UserId = newUser.Id
                    })];
                await _taskListRepository.AddRangeAsync(defaultTaskLists, cancellationToken);

                List<Category> defaultCategories = [.. NewUserConfiguration.DefaultCategories
                    .Select(category => new Category
                    {
                        Name = category.Name,
                        ColorHex = category.ColorHex,
                        Default = true,
                        UserId = newUser.Id
                    })];
                await _categoryRepository.AddRangeAsync(defaultCategories, cancellationToken);

                await _unitOfWork.CommitAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync(CancellationToken.None);
                throw new InternalException("An error occurred while creating the user.", ex);
            }
        }

        public async Task DeleteAsync(string userId, CancellationToken cancellationToken = default)
        {
            var userGuid = userId.ParseToGuidWithThrow("user");

            if (!await _userRepository.ExistsAsync(u => u.Id == userGuid, cancellationToken))
                throw new NotFoundException("user");

            await _userRepository.DeleteAsync(userGuid, cancellationToken);
        }

        public async Task<UserDto?> GetAsync(string userId, bool includeCategories = false, bool includeTasks = false, bool includeTaskLists = false, CancellationToken cancellationToken = default)
        {
            var userGuid = userId.ParseToGuidWithThrow("user");

            var includes = new List<Expression<Func<User, object>>>();
            if (includeCategories) includes.Add(u => u.Categories);
            if (includeTasks) includes.Add(u => u.Tasks);
            if (includeTaskLists) includes.Add(u => u.TaskLists);

            var user = await _userRepository.FindAsync(
                predicate: u => u.Id == userGuid,
                cancellationToken: cancellationToken,
                includes: [.. includes]
            ) ?? throw new NotFoundException("user");

            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto?> GetByEmailAsync(string email, bool includeCategories = false, bool includeTasks = false, bool includeTaskLists = false, CancellationToken cancellationToken = default)
        {
            var includes = new List<Expression<Func<User, object>>>();
            if (includeCategories) includes.Add(u => u.Categories);
            if (includeTasks) includes.Add(u => u.Tasks);
            if (includeTaskLists) includes.Add(u => u.TaskLists);

            var user = await _userRepository.FindAsync(
                predicate: u => u.Email == email,
                includes: [.. includes],
                cancellationToken: cancellationToken
            ) ?? throw new NotFoundException("user");

            return _mapper.Map<UserDto>(user);
        }

        public async Task<PagedResult<UserDto>> ListAsync(string userId, int page = 0, int pageSize = 10, UserSortBy userSortBy = UserSortBy.FullName, bool includeTasks = false, bool includeCategory = false, bool includeTaskLists = false, CancellationToken cancellationToken = default)
        {
            var userGuid = userId.ParseToGuidWithThrow("user");

            PaginationHelper.ValidatePageSettings(page, pageSize);

            var includes = new List<Expression<Func<User, object>>>();
            if (includeCategory) includes.Add(u => u.Categories);
            if (includeTasks) includes.Add(u => u.Tasks);
            if (includeTaskLists) includes.Add(u => u.TaskLists);

            var orderBy = userSortBy switch
            {
                UserSortBy.FullName => (Expression<Func<User, object>>)(u => u.FullName),
                UserSortBy.Email => u => u.Email,
                UserSortBy.CreatedAt => u => u.CreatedAt,
                UserSortBy.UpdatedAt => u => u.UpdatedAt,
                _ => throw new ArgumentOutOfRangeException(nameof(userSortBy), userSortBy, "Invalid sort option.")
            };

            var result = await _userRepository.ListAsync(
                predicate: u => u.Id == userGuid,
                page: page,
                pageSize: pageSize,
                cancellationToken: cancellationToken,
                includes: [.. includes]
            );

            return new PagedResult<UserDto>(
                _mapper.Map<IReadOnlyCollection<UserDto>>(result.Data),
                result.Count,
                result.CurrentPage,
                result.TotalPages);
        }
    }
}
