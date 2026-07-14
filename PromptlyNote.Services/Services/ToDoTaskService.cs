using AutoMapper;
using LinqKit;
using PromptlyNote.Core.Constants;
using PromptlyNote.Core.DTOs;
using PromptlyNote.Core.DTOs.Forms.Create;
using PromptlyNote.Core.DTOs.Forms.Update;
using PromptlyNote.Core.DTOs.LightDTOs;
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
    public class ToDoTaskService(
        IToDoTaskRepository taskRepository,
        ICategoryRepository categoryRepository,
        IUserRepository userRepository,
        ITaskListRepository taskListRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IGoogleCalendarService googleCalendarService) : IToDoTaskService
    {
        private readonly IToDoTaskRepository _taskRepository = taskRepository;
        private readonly ICategoryRepository _categoryRepository = categoryRepository;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly ITaskListRepository _taskListRepository = taskListRepository;
        private readonly IGoogleCalendarService _googleCalendarService = googleCalendarService;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;

        public async Task CreateAsync(CreateToDoTaskForm form, string userId, CancellationToken cancellationToken = default)
        {
            var userGuid = userId.ParseToGuidWithThrow("user");
            var categoryGuid = form.CategoryId?.ParseToGuidWithThrow("category");
            var taskListGuid = form.TaskListId.ParseToGuidWithThrow("task list");

            if (!await _userRepository.ExistsAsync(u => u.Id == userGuid, cancellationToken))
                throw new NotFoundException("user");

            if (categoryGuid is not null)
            {
                var category = await _categoryRepository.FindAsync(
                    predicate: c => c.Id == categoryGuid,
                    selector: c => new { c.UserId },
                    cancellationToken: cancellationToken
                ) ?? throw new NotFoundException("category");

                if (category.UserId != userGuid)
                    throw new ForbiddenException(ExceptionMessages.NotOwner("category"));
            }

            var taskList = await _taskListRepository.FindAsync(
                predicate: tl => tl.Id == taskListGuid,
                selector: tl => new { tl.UserId },
                cancellationToken: cancellationToken
            ) ?? throw new NotFoundException("task list");

            if (taskList.UserId != userGuid)
                throw new ForbiddenException(ExceptionMessages.NotOwner("task list"));

            var subTasks = _mapper.Map<List<SubTask>>(form.SubTasks);
            for (int i = 0; i < subTasks.Count; i++)
            {
                subTasks[i].Order = i + 1;
            }

            var task = new ToDoTask
            {
                Name = form.Name,
                Note = form.Note,
                DueDate = form.DueDate,
                CategoryId = categoryGuid,
                TaskListId = taskListGuid,
                UserId = userGuid,
                SubTasks = _mapper.Map<List<SubTask>>(form.SubTasks),
                RemindBeforeMinutes = form.RemindBeforeMinutes,
                SyncToGoogleCalendar = form.SyncToGoogleCalendar
            };

            if (form.SyncToGoogleCalendar && form.DueDate is null)
                throw new ArgumentException("Task does not have a due date to sync to Google Calendar.");

            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                await _taskRepository.AddAsync(task, cancellationToken);

                if (form.SyncToGoogleCalendar)
                    await _googleCalendarService.CreateEventAsync(userId, _mapper.Map<ToDoTaskLightDto>(task), cancellationToken);

                await _unitOfWork.CommitAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync(CancellationToken.None);
                throw new InternalException("An error occurred while creating the task.", ex);
            }
        }

        public async Task UpdateAsync(string taskId, string userId, UpdateToDoTaskForm form, CancellationToken cancellationToken = default)
        {
            var taskGuid = taskId.ParseToGuidWithThrow("task");
            var userGuid = userId.ParseToGuidWithThrow("user");
            var categoryGuid = form.CategoryId?.ParseToGuidWithThrow("category");
            var taskListGuid = form.TaskListId.ParseToGuidWithThrow("task list");

            var task = await _taskRepository.FindAsync(
                predicate: t => t.Id == taskGuid,
                cancellationToken: cancellationToken
            ) ?? throw new NotFoundException("task");

            if (task.UserId != userGuid)
                throw new ForbiddenException(ExceptionMessages.NotOwner("task"));

            var wasInCalendar = task.SyncToGoogleCalendar && !task.IsCompleted;
            var shouldBeInCalendar = form.SyncToGoogleCalendar && !form.IsCompleted;

            if (shouldBeInCalendar && form.DueDate is null)
                throw new ArgumentException("Task does not have a due date to sync to Google Calendar.");

            var subTasks = _mapper.Map<List<SubTask>>(form.SubTasks);
            for (int i = 0; i < subTasks.Count; i++)
            {
                subTasks[i].Order = i + 1;
            }

            task.Name = form.Name;
            task.Note = form.Note;
            task.DueDate = form.DueDate;
            task.CategoryId = categoryGuid;
            task.TaskListId = taskListGuid;
            task.IsCompleted = form.IsCompleted;
            task.SubTasks = _mapper.Map<List<SubTask>>(form.SubTasks);
            task.RemindBeforeMinutes = form.RemindBeforeMinutes;
            task.SyncToGoogleCalendar = form.SyncToGoogleCalendar;

            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                await _taskRepository.UpdateAsync(task);

                if (shouldBeInCalendar)
                {
                    if (wasInCalendar)
                        await _googleCalendarService.DeleteEventAsync(userId, task.Id.ToString(), cancellationToken);

                    await _googleCalendarService.CreateEventAsync(userId, _mapper.Map<ToDoTaskLightDto>(task), cancellationToken);
                }
                else if (wasInCalendar)
                {
                    await _googleCalendarService.DeleteEventAsync(userId, task.Id.ToString(), cancellationToken);
                }

                await _unitOfWork.CommitAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync(CancellationToken.None);
                throw new InternalException("An error occurred while updating the task.", ex);
            }
        }

        public async Task DeleteAsync(string taskId, string userId, CancellationToken cancellationToken = default)
        {
            var taskGuid = taskId.ParseToGuidWithThrow("task");
            var userGuid = userId.ParseToGuidWithThrow("user");

            var task = await _taskRepository.FindAsync(
                predicate: t => t.Id == taskGuid,
                selector: t => new { t.UserId },
                cancellationToken: cancellationToken
            ) ?? throw new NotFoundException("task");

            if (task.UserId != userGuid)
                throw new ForbiddenException(ExceptionMessages.NotOwner("task"));

            await _taskRepository.DeleteAsync(taskGuid, cancellationToken);
        }

        public async Task<PagedResult<ToDoTaskDto>> ListAsync(string userId, int page = PaginationConfiguration.MinimumPage, int pageSize = PaginationConfiguration.DefaultPageSize, ToDoTaskSortBy toDoTaskSortBy = ToDoTaskSortBy.Name, bool includeCategory = false, bool includeTaskList = false, string? categoryFilter = null, string? taskListFilter = null, CancellationToken cancellationToken = default)
        {
            var userGuid = userId.ParseToGuidWithThrow("user");

            PaginationHelper.ValidatePageSettings(page, pageSize);

            var includes = new List<Expression<Func<ToDoTask, object>>>();
            if (includeCategory) includes.Add(t => t.Category!);
            if (includeTaskList) includes.Add(t => t.TaskList!);
            includes.Add(t => t.SubTasks);

            var orderBy = toDoTaskSortBy switch
            {
                ToDoTaskSortBy.Name => (Expression<Func<ToDoTask, object>>)(t => t.Name),
                ToDoTaskSortBy.DueDate => t => t.DueDate!,
                ToDoTaskSortBy.IsCompleted => t => t.IsCompleted,
                ToDoTaskSortBy.CreatedAt => t => t.CreatedAt,
                ToDoTaskSortBy.UpdatedAt => t => t.UpdatedAt,
                _ => t => t.Id
            };

            var predicate = PredicateBuilder.New<ToDoTask>(t => t.UserId == userGuid);

            if (categoryFilter != null)
            {
                var categoryGuid = categoryFilter.ParseToGuidWithThrow("category");
                predicate = predicate.And(t => t.CategoryId == categoryGuid);
            }

            if (taskListFilter != null)
            {
                var taskListGuid = taskListFilter.ParseToGuidWithThrow("task list");
                predicate = predicate.And(t => t.TaskListId == taskListGuid);
            }

            var result = await _taskRepository.ListAsync(
                predicate: predicate,
                page: page,
                pageSize: pageSize,
                orderBy: orderBy,
                cancellationToken: cancellationToken,
                includes: [.. includes]
            );

            foreach (var item in result.Data)
            {
                item.SubTasks = [.. item.SubTasks.OrderBy(st => st.Order)];
            }

            return new PagedResult<ToDoTaskDto>(_mapper.Map<List<ToDoTaskDto>>(result.Data), result.Count, result.CurrentPage, result.TotalPages);
        }

        public async Task<ToDoTaskDto> GetAsync(string toDoTaskId, string userId, bool includeCategory = false, bool includeTaskList = false, CancellationToken cancellationToken = default)
        {
            var taskGuid = toDoTaskId.ParseToGuidWithThrow("task");
            var userGuid = userId.ParseToGuidWithThrow("user");

            var includes = new List<Expression<Func<ToDoTask, object>>>();
            if (includeCategory) includes.Add(t => t.Category!);
            if (includeTaskList) includes.Add(t => t.TaskList!);
            includes.Add(t => t.SubTasks);

            var task = await _taskRepository.FindAsync(
                predicate: t => t.Id == taskGuid,
                cancellationToken: cancellationToken,
                includes: [.. includes]
            ) ?? throw new NotFoundException("task");

            if (task.UserId != userGuid)
                throw new ForbiddenException(ExceptionMessages.NotOwner("task"));

            task.SubTasks = [.. task.SubTasks.OrderBy(st => st.Order)];

            return _mapper.Map<ToDoTaskDto>(task);
        }

        public async Task ReplaceSubTasksAsync(string taskId, string userId, List<CreateSubTaskForm> subTasks, CancellationToken cancellationToken = default)
        {
            var taskGuid = taskId.ParseToGuidWithThrow("task");
            var userGuid = userId.ParseToGuidWithThrow("user");

            var task = await _taskRepository.FindAsync(
                predicate: t => t.Id == taskGuid,
                selector: t => new { t.UserId },
                cancellationToken: cancellationToken
            ) ?? throw new NotFoundException("task");

            if (task.UserId != userGuid)
                throw new ForbiddenException(ExceptionMessages.NotOwner("task"));

            var orderedSubTasks = _mapper.Map<List<SubTask>>(subTasks);
            for (int i = 0; i < subTasks.Count; i++)
            {
                orderedSubTasks[i].Order = i + 1;
            }

            await _taskRepository.ReplaceSubTasksAsync(taskGuid, orderedSubTasks, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task AddToCalendar(string taskId, string userId, CancellationToken cancellationToken = default)
        {
            var taskGuid = taskId.ParseToGuidWithThrow("task");
            var userGuid = userId.ParseToGuidWithThrow("user");

            var task = await _taskRepository.FindAsync(
                predicate: t => t.Id == taskGuid,
                cancellationToken: cancellationToken
            ) ?? throw new NotFoundException("task");

            if (task.UserId != userGuid)
                throw new ForbiddenException(ExceptionMessages.NotOwner("task"));

            if (task.DueDate is null)
                throw new ArgumentException("Task does not have a due date to add to calendar.");

            task.SyncToGoogleCalendar = true;
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                await _taskRepository.UpdateAsync(task);
                await _googleCalendarService.DeleteEventAsync(userId, taskId, cancellationToken);
                await _googleCalendarService.CreateEventAsync(userId, _mapper.Map<ToDoTaskLightDto>(task), cancellationToken);
                await _unitOfWork.CommitAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync(CancellationToken.None);
                throw new InternalException("An error occurred while adding the task to the calendar.", ex);
            }
        }

        public async Task RemoveFromCalendar(string taskId, string userId, CancellationToken cancellationToken = default)
        {
            var taskGuid = taskId.ParseToGuidWithThrow("task");
            var userGuid = userId.ParseToGuidWithThrow("user");
            var task = await _taskRepository.FindAsync(
                predicate: t => t.Id == taskGuid,
                cancellationToken: cancellationToken
            ) ?? throw new NotFoundException("task");
            if (task.UserId != userGuid)
                throw new ForbiddenException(ExceptionMessages.NotOwner("task"));

            task.SyncToGoogleCalendar = false;
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                await _taskRepository.UpdateAsync(task);
                await _googleCalendarService.DeleteEventAsync(userId, taskId, cancellationToken);
                await _unitOfWork.CommitAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync(CancellationToken.None);
                throw new InternalException("An error occurred while removing the task from the calendar.", ex);
            }
        }
    }
}
