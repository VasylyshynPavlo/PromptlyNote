using Microsoft.EntityFrameworkCore;
using PromptlyNote.Core.Constants;
using PromptlyNote.Core.Entities;
using PromptlyNote.Core.Models;
using PromptlyNote.Core.Utils;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace PromptlyNote.Data
{
    public class ApplicationDbSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            await context.Database.EnsureCreatedAsync();
            await SeedDefaultUserAsync(context);
        }

        private static async Task SeedDefaultUserAsync(ApplicationDbContext context)
        {
            if (await context.Users.Where(u => u.Email == "defaultuser@example.com").AnyAsync())
                return;

            using var transaction = await context.Database.BeginTransactionAsync();
            User defaultUser = new()
            {
                FullName = "Default User",
                Email = "defaultuser@example.com",
                PasswordHash = PasswordHesher.HashPassword("admin123"),
                AvatarUrl = "https://i.pravatar.cc/150?u=defaultuser@example.com",
            };
            await context.Users.AddAsync(defaultUser);
            await context.SaveChangesAsync();
            var taskLists = new List<TaskList>();
            foreach (var item in NewUserConfiguration.DefaultList)
            {
                var taskList = new TaskList
                {
                    Name = item.Name,
                    Description = item.Description,
                    IconName = item.IconName,
                    Default = true,
                    UserId = defaultUser.Id
                };
                taskLists.Add(taskList);
            }
            await context.TaskLists.AddRangeAsync(taskLists);
            var categories = new List<Category>();
            foreach (var item in NewUserConfiguration.DefaultCategories)
            {
                var category = new Category
                {
                    Name = item.Name,
                    ColorHex = item.ColorHex,
                    UserId = defaultUser.Id,
                    Default = true,
                };
                categories.Add(category);
            }
            await context.Categories.AddRangeAsync(categories);
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
    }
}
