using AutoMapper;
using Google.Apis.Auth;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Microsoft.Extensions.Configuration;
using PromptlyNote.Core.Constants;
using PromptlyNote.Core.DTOs;
using PromptlyNote.Core.DTOs.Forms.Auth;
using PromptlyNote.Core.DTOs.LightDTOs;
using PromptlyNote.Core.Entities;
using PromptlyNote.Core.Exceptions;
using PromptlyNote.Core.Interfaces;
using PromptlyNote.Core.Interfaces.Repositories;
using PromptlyNote.Core.Interfaces.Services;
using PromptlyNote.Core.Utils;

namespace PromptlyNote.Services.Services
{
    public class AuthService(
        IJwtService jwtService,
        IUserRepository userRepository,
        ITaskListRepository taskListRepository,
        ICategoryRepository categoryRepository,
        IConfiguration configuration,
        IMapper mapper,
        IUnitOfWork unitOfWork) : IAuthService
    {
        private readonly IJwtService _jwtService = jwtService;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly ITaskListRepository _taskListRepository = taskListRepository;
        private readonly ICategoryRepository _categoryRepository = categoryRepository;
        private readonly IMapper _mapper = mapper;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        private readonly string _clientId = configuration["Authentication:Google:ClientId"]
            ?? throw new ArgumentNullException("Google ClientId is not configured.");
        private readonly string _clientSecret = configuration["Authentication:Google:ClientSecret"]
            ?? throw new ArgumentNullException("Google ClientSecret is not configured.");

        public async Task<(string accessToken, UserDto userDto)> RegisterAsync(RegisterForm registerForm, CancellationToken cancellationToken = default)
        {
            var user = await CreateUserWithDefaultAsync(registerForm.FullName, registerForm.Email, null, PasswordHesher.HashPassword(registerForm.Password), cancellationToken);
            return (_jwtService.GenerateAccessToken(user), _mapper.Map<UserDto>(user));
        }

        public async Task<(string accessToken, UserDto userDto)> LoginAsync(LoginForm loginForm, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.FindAsync(u => u.Email == loginForm.Email, cancellationToken: cancellationToken)
                ?? throw new ArgumentException("Invalid credentials.");

            if (user.PasswordHash is null)
            {
                throw new ArgumentException("Invalid credentials.");
            }

            if (!PasswordHesher.Verify(loginForm.Password, user.PasswordHash))
            {
                throw new ArgumentException("Invalid credentials.");
            }

            return (_jwtService.GenerateAccessToken(user), _mapper.Map<UserDto>(user));
        }

        public async Task<(string accessToken, UserDto userDto)> AuthViaGoogleAsync(string code, string redirectUri, CancellationToken cancellationToken = default)
        {
            using var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new ClientSecrets
                {
                    ClientId = _clientId,
                    ClientSecret = _clientSecret
                },
                Scopes = ["openid", "email", "profile"]
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
                throw new ArgumentException("Invalid Google token.", ex);
            }

            if (!payload.EmailVerified)
            {
                throw new ArgumentException("Email is not verified.");
            }

            var user = await _userRepository.FindAsync(u => u.GoogleSub == payload.Subject, cancellationToken: cancellationToken);
            if (user is null)
            {
                var existingByEmail = await _userRepository.FindAsync(u => u.Email == payload.Email, cancellationToken: cancellationToken);
                if (existingByEmail is not null)
                {
                    existingByEmail.GoogleSub = payload.Subject;
                    await _userRepository.UpdateAsync(existingByEmail);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    user = existingByEmail;
                }
                else
                {
                    user = await CreateUserWithDefaultAsync(payload.Name, payload.Email, payload.Subject, null, cancellationToken);
                }
            }

            return (_jwtService.GenerateAccessToken(user), _mapper.Map<UserDto>(user));
        }

        private async Task<User> CreateUserWithDefaultAsync(string fullName, string email, string? googleSub, string? passwordHash, CancellationToken cancellationToken)
        {
            if (await _userRepository.ExistsAsync(u => u.Email == email, cancellationToken: cancellationToken))
            {
                throw new ConflictException(ExceptionMessages.ConflictFieldsName("user", "email"));
            }
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                User newUser = new()
                {
                    FullName = fullName,
                    Email = email,
                    PasswordHash = passwordHash,
                    GoogleSub = googleSub
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
                return newUser;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync(CancellationToken.None);
                throw new InternalException("An error occurred while creating the user.", ex);
            }
        }
    }
}
