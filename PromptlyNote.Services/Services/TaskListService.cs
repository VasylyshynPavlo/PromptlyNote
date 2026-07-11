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
    public class TaskListService(
        ITaskListRepository taskListRepository,
        IMapper mapper,
        IUnitOfWork unitOfWork) : ITaskListService
    {
        private readonly ITaskListRepository _taskListRepository = taskListRepository;
        private readonly IMapper _mapper = mapper;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task CreateAsync(CreateTaskListForm form, string userId, CancellationToken cancellationToken = default)
        {
            var userGuid = userId.ParseToGuidWithThrow("user");

            TaskList taskList = new()
            {
                Name = form.Name,
                IconName = form.IconName,
                UserId = userGuid,
                Description = form.Description,
            };

            await _taskListRepository.AddAsync(taskList, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(string taskListId, string userId, CancellationToken cancellationToken = default)
        {
            var taskListGuid = taskListId.ParseToGuidWithThrow("task list");
            var userGuid = userId.ParseToGuidWithThrow("user");

            var taskList = await _taskListRepository.FindAsync(
                predicate: tl => tl.Id == taskListGuid,
                selector: tl => new { tl.UserId, tl.Default, tl.Id },
                cancellationToken: cancellationToken
            ) ?? throw new NotFoundException("task list");

            if (taskList.UserId != userGuid)
                throw new ForbiddenException(ExceptionMessages.NoPermission("delete", "task list"));

            if (taskList.Default)
                throw new ForbiddenException(ExceptionMessages.NoPermission("delete", "default task list"));

            await _taskListRepository.DeleteAsync(taskListGuid, cancellationToken);
        }

        public async Task<PagedResult<TaskListDto>> ListAsync(string userId, int page = PaginationConfiguration.MinimumPage, int pageSize = PaginationConfiguration.DefaultPageSize, TaskListSortBy taskListSortBy = TaskListSortBy.Name, bool includeTasks = false, CancellationToken cancellationToken = default)
        {
            var userGuid = userId.ParseToGuidWithThrow("user");

            PaginationHelper.ValidatePageSettings(page, pageSize);

            var includes = new List<Expression<Func<TaskList, object>>>();
            if (includeTasks)
                includes.Add(tl => tl.Tasks);

            var orderBy = taskListSortBy switch
            {
                TaskListSortBy.Name => (Expression<Func<TaskList, object>>)(tl => tl.Name),
                TaskListSortBy.Default => tl => tl.Default,
                TaskListSortBy.CreatedAt => tl => tl.CreatedAt,
                TaskListSortBy.UpdatedAt => tl => tl.UpdatedAt,
                _ => tl => tl.Id
            };

            var result = await _taskListRepository.ListAsync(
                predicate: tl => tl.UserId == userGuid,
                page: page,
                pageSize: pageSize,
                orderBy: orderBy,
                cancellationToken: cancellationToken,
                includes: [.. includes]
            );

            Console.WriteLine($"UserId: {userGuid}");
            Console.WriteLine($"Page: {page}, PageSize: {pageSize}");
            Console.WriteLine($"Fetched task list count: {result.Data.Count}");

            return new PagedResult<TaskListDto>(
                _mapper.Map<IReadOnlyCollection<TaskListDto>>(result.Data),
                result.Count,
                result.CurrentPage,
                result.TotalPages
            );
        }

        public async Task<TaskListDto?> GetAsync(string taskListId, string userId, bool includeTasks = false, CancellationToken cancellationToken = default)
        {
            var taskListGuid = taskListId.ParseToGuidWithThrow("task list");
            var userGuid = userId.ParseToGuidWithThrow("user");

            var includes = new List<Expression<Func<TaskList, object>>>();

            if (includeTasks)
                includes.Add(tl => tl.Tasks);

            var taskList = await _taskListRepository.FindAsync(
                predicate: tl => tl.Id == taskListGuid,
                cancellationToken: cancellationToken,
                includes: [.. includes]
            ) ?? throw new NotFoundException("task list");

            return taskList.UserId != userGuid
                ? throw new ForbiddenException(ExceptionMessages.NoPermission("access", "task list"))
                : _mapper.Map<TaskListDto>(taskList);
        }

        public async Task UpdateAsync(string taskListId, string userId, UpdateTaskListForm form, CancellationToken cancellationToken = default)
        {
            var taskListGuid = taskListId.ParseToGuidWithThrow("task list");
            Console.WriteLine($"Updating task list: {taskListGuid}");
            var userGuid = userId.ParseToGuidWithThrow("user");

            var taskList = await _taskListRepository.FindAsync(
                predicate: tl => tl.Id == taskListGuid,
                cancellationToken: cancellationToken
            ) ?? throw new NotFoundException("task list");

            if (taskList.UserId != userGuid)
                throw new ForbiddenException(ExceptionMessages.NotOwner("task list"));

            if (taskList.Default)
                throw new ForbiddenException(ExceptionMessages.NoPermission("update", "default task list"));

            taskList.Name = form.Name;
            taskList.Description = form.Description;
            taskList.IconName = form.IconName;
            taskList.UpdatedAt = DateTime.UtcNow;

            await _taskListRepository.UpdateAsync(taskList);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
