using PromptlyNote.Core.Constants;
using PromptlyNote.Core.DTOs;
using PromptlyNote.Core.DTOs.Forms.Create;
using PromptlyNote.Core.DTOs.Forms.Update;
using PromptlyNote.Core.Entities;
using PromptlyNote.Core.Enums;
using PromptlyNote.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PromptlyNote.Core.Interfaces.Services
{
    public interface IToDoTaskService
    {
        Task CreateAsync(CreateToDoTaskForm form, string userId, CancellationToken cancellationToken = default);
        Task UpdateAsync(string taskId, string userId, UpdateToDoTaskForm form, CancellationToken cancellationToken = default);
        Task DeleteAsync(string taskId, string userId, CancellationToken cancellationToken = default);
        Task<PagedResult<ToDoTaskDto>> ListAsync(string userId, int page = PaginationConfiguration.MinimumPage, int pageSize = PaginationConfiguration.DefaultPageSize, ToDoTaskSortBy toDoTaskSortBy = ToDoTaskSortBy.Name, bool includeCategory = false, bool includeTaskList = false, bool includeSubTasks = false, CancellationToken cancellationToken = default);
        Task<ToDoTaskDto> GetAsync(string taskId, string userId, bool includeCategory = false, bool includeTaskList = false, bool includeSubTasks = false, CancellationToken cancellationToken = default);

        Task AddSubTaskAsync(string taskId, string userId, CreateSubTaskForm subTask, CancellationToken cancellationToken = default);
        Task DeleteSubTaskAsync(string subTaskId, string taskId, string userId, CancellationToken cancellationToken = default);
        Task ReplaceSubTasksAsync(string taskId, string userId, List<CreateSubTaskForm> subTasks, CancellationToken cancellationToken = default);
        Task UpdateSubTaskAsync(string taskId, string userId, UpdateSubTaskForm subTask, CancellationToken cancellationToken = default);
    }
}
