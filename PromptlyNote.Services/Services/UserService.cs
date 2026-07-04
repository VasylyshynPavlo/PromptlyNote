using PromptlyNote.Core.Constants;
using PromptlyNote.Core.DTOs.Forms;
using PromptlyNote.Core.Entities;
using PromptlyNote.Core.Interfaces;
using PromptlyNote.Core.Interfaces.Repositories;
using PromptlyNote.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace PromptlyNote.Services.Services
{
    public class UserService(IUserRepository userRepository, ITaskListRepository taskListRepository, ICategoryRepository categoryRepository, IUnitOfWork unitOfWork) : IUserService
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ICategoryRepository _categoryRepository = categoryRepository;
        private readonly ITaskListRepository _taskListRepository = taskListRepository;

        public async Task ChangeFullName(string fullName, string userId)
        {
            if (!Guid.TryParse(userId, out var userGuid))
                throw new ArgumentException("Invalid user ID format.", nameof(userId));
            var user = await _userRepository.GetAsync(userGuid) ?? throw new KeyNotFoundException("User not found.");
            if (user.FullName == fullName)
                throw new InvalidOperationException("The new full name is the same as the current one.");
            user.FullName = fullName;
            await _userRepository.UpdateAsync(user);
        }

        public async Task CreateUser(CreateUserForm form, bool viaGoogle)
        {
            if (await _userRepository.ExistsAsync(u => u.Email == form.Email))
                throw new InvalidOperationException("A user with this email already exists.");
            if (form.Password != null && viaGoogle)
                throw new InvalidOperationException("Password should not be provided when registering via Google.");
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                User newUser = new()
                {
                    AvatarUrl = form.AvatarUrl,
                    FullName = form.FullName,
                    PasswordHash = form.Password != null ? BCrypt.Net.BCrypt.HashPassword(form.Password) : null,
                    GoogleAuth = viaGoogle,
                    Email = form.Email
                };
                await _userRepository.AddAsync(newUser);
                List<TaskList> defaultTaskLists = [.. NewUserConfiguration.DefaultList
                    .Select(taskList => new TaskList
                    {
                        Name = taskList.Name,
                        IconName = taskList.IconName,
                        Description = taskList.Description,
                        Default = true,
                        UserId = newUser.Id
                    })];
                await _taskListRepository.AddRangeAsync(defaultTaskLists);
                List<Category> defaultCategories = [.. NewUserConfiguration.DefaultCategories
                    .Select(category => new Category
                    {
                        Name = category.Name,
                        ColorHex = category.ColorHex,
                        Default = true,
                        UserId = newUser.Id
                    })];
                await _categoryRepository.AddRangeAsync(defaultCategories);
                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw new InternalException("An error occurred while creating the user.", ex);
            }
        }

        public async Task DeleteUser(string id, string userId)
        {
            throw new NotImplementedException();
        }
    }
}
