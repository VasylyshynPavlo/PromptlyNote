using AutoMapper;
using Google.Apis.Auth;
using Google.Apis.Auth.OAuth2.Flows;
using Microsoft.Extensions.Configuration;
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
        IMapper mapper,
        IGoogleCalendarService googleCalendarService,
        IConfiguration configuration) : IUserService
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ICategoryRepository _categoryRepository = categoryRepository;
        private readonly ITaskListRepository _taskListRepository = taskListRepository;
        private readonly IGoogleCalendarService _googleCalendarService = googleCalendarService;
        private readonly IMapper _mapper = mapper;

        private readonly string _clientId = configuration["Authentication:Google:ClientId"]
            ?? throw new ArgumentNullException("Google ClientId is not configured.");
        private readonly string _clientSecret = configuration["Authentication:Google:ClientSecret"]
            ?? throw new ArgumentNullException("Google ClientSecret is not configured.");

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
                throw new BadRequestException("Password should not be provided when registering via Google.");

            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                User newUser = new()
                {
                    FullName = form.FullName,
                    PasswordHash = form.Password is not null ? BCrypt.Net.BCrypt.HashPassword(form.Password) : null,
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

            await _googleCalendarService.DisconnectAsync(userId, cancellationToken);

            await _userRepository.DeleteAsync(userGuid, cancellationToken);
        }

        public async Task<UserDto?> GetAsync(string userId, CancellationToken cancellationToken = default)
        {
            var userGuid = userId.ParseToGuidWithThrow("user");

            var user = await _userRepository.FindAsync(
                predicate: u => u.Id == userGuid,
                cancellationToken: cancellationToken
            ) ?? throw new NotFoundException("user");

            var dto = _mapper.Map<UserDto>(user);
            dto.GoogleCalendar = await _googleCalendarService.IsConnectedAsync(userId, cancellationToken);

            return dto;
        }

        public async Task<UserDto?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.FindAsync(
                predicate: u => u.Email == email,
                cancellationToken: cancellationToken
            ) ?? throw new NotFoundException("user");

            var dto = _mapper.Map<UserDto>(user);
            dto.GoogleCalendar = await _googleCalendarService.IsConnectedAsync(user.Id.ToString(), cancellationToken);

            return dto;
        }

        public async Task<PagedResult<UserDto>> ListAsync(string userId, int page = 0, int pageSize = 10, UserSortBy userSortBy = UserSortBy.FullName, bool isDescending = false, CancellationToken cancellationToken = default)
        {
            var userGuid = userId.ParseToGuidWithThrow("user");

            PaginationHelper.ValidatePageSettings(page, pageSize);

            var orderBy = userSortBy switch
            {
                UserSortBy.FullName => (Expression<Func<User, object>>)(u => u.FullName),
                UserSortBy.Email => u => u.Email,
                UserSortBy.CreatedAt => u => u.CreatedAt,
                UserSortBy.UpdatedAt => u => u.UpdatedAt,
                _ => throw new BadRequestException("Invalid sort option."),
            };

            var result = await _userRepository.ListAsync(
                predicate: u => u.Id == userGuid,
                page: page,
                pageSize: pageSize,
                orderBy: orderBy,
                isDescending: isDescending,
                cancellationToken: cancellationToken
            );

            var userDtos = _mapper.Map<IReadOnlyCollection<UserDto>>(result.Data);
            foreach (var userDto in userDtos)
            {
                userDto.GoogleCalendar = await _googleCalendarService.IsConnectedAsync(userId, cancellationToken);
            }

            return new PagedResult<UserDto>(
                userDtos,
                result.Count,
                result.CurrentPage,
                result.TotalPages);
        }

        public async Task ChangePasswordAsync(string userId, string oldPassword, string newPassword, CancellationToken cancellationToken = default)
        {
            var userGuid = userId.ParseToGuidWithThrow("user");

            var user = await _userRepository.FindAsync(
                predicate: u => u.Id == userGuid,
                cancellationToken: cancellationToken
            ) ?? throw new NotFoundException("user");

            if (user.PasswordHash is null || !BCrypt.Net.BCrypt.Verify(oldPassword, user.PasswordHash))
            {
                throw new BadRequestException("Invalid credentials.");
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _userRepository.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task SetPassword(string code, string newPassword, string redirectUri, CancellationToken cancellationToken = default)
        {
            using var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new Google.Apis.Auth.OAuth2.ClientSecrets
                {
                    ClientId = _clientId,
                    ClientSecret = _clientSecret
                },
                Scopes = ["openid", "email"]
            });

            var tokenResponse = await flow.ExchangeCodeForTokenAsync(
                userId: "user",
                code: code,
                redirectUri: redirectUri,
                CancellationToken.None
            );

            var settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = [_clientId]
            };

            GoogleJsonWebSignature.Payload payload;
            try
            {
                payload = await GoogleJsonWebSignature.ValidateAsync(tokenResponse.IdToken, settings);
            }
            catch (InvalidJwtException ex)
            {
                throw new ForbiddenException("Invalid Google token.", ex);
            }

            if (!payload.EmailVerified)
            {
                throw new ForbiddenException("Email is not verified.");
            }

            var user = await _userRepository.FindAsync(
                predicate: u => u.Email == payload.Email,
                cancellationToken: cancellationToken
            ) ?? throw new NotFoundException("user");

            if (user.PasswordHash is not null)
            {
                throw new ConflictException("Password has already been set.");
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);

            await _userRepository.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
