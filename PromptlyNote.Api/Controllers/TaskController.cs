using Google.Apis.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PromptlyNote.Core.Constants;
using PromptlyNote.Core.DTOs.Forms.Create;
using PromptlyNote.Core.DTOs.Forms.Update;
using PromptlyNote.Core.Enums;
using PromptlyNote.Core.Interfaces.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;

namespace PromptlyNote.Api.Controllers
{
    [Authorize]
    [Route("api/task")]
    [ApiController]
    public class TaskController(IToDoTaskService toDoTaskService, IUserService userService) : ControllerBase
    {
        private readonly IToDoTaskService _toDoTaskService = toDoTaskService;
        private readonly IUserService _userService = userService;

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id, bool includeCategory = false, bool includeTaskList = false, CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                      ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
            {
                return Unauthorized();
            }

            return Ok(await _toDoTaskService.GetAsync(id, userId, includeCategory, includeTaskList, cancellationToken));
        }

        [HttpGet]
        public async Task<IActionResult> List(int page = PaginationConfiguration.MinimumPage, int pageSize = PaginationConfiguration.DefaultPageSize, ToDoTaskSortBy toDoTaskSortBy = ToDoTaskSortBy.Name, bool isDescending = false, bool includeCategory = false, bool includeTaskList = false, string? categoryFilter = null, string? taskListFilter = null, CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                      ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
            {
                return Unauthorized();
            }

            return Ok(await _toDoTaskService.ListAsync(userId, page, pageSize, toDoTaskSortBy, isDescending, includeCategory, includeTaskList, categoryFilter, taskListFilter, cancellationToken));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateToDoTaskForm form, CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                      ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
            {
                return Unauthorized();
            }

            Console.WriteLine($"Form: {JsonSerializer.Serialize(form)}");

            await _toDoTaskService.CreateAsync(form, userId, cancellationToken);
            return Created();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateToDoTaskForm form, CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                      ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
            {
                return Unauthorized();
            }

            await _toDoTaskService.UpdateAsync(id, userId, form, cancellationToken);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                      ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
            {
                return Unauthorized();
            }

            await _toDoTaskService.DeleteAsync(id, userId, cancellationToken);
            return NoContent();
        }

        [HttpPost("{taskId}/calendar")]
        public async Task<IActionResult> AddToCalendar(string taskId, CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                      ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
            {
                return Unauthorized();
            }
            await _toDoTaskService.AddToCalendar(taskId, userId, cancellationToken);
            return NoContent();
        }

        [HttpDelete("{taskId}/calendar")]
        public async Task<IActionResult> DeleteFromCalendar(string taskId, CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                      ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
            {
                return Unauthorized();
            }
            await _toDoTaskService.RemoveFromCalendar(taskId, userId, cancellationToken);
            return NoContent();
        }

        [HttpPut("{taskId}/subtasks")]
        public async Task<IActionResult> ReplaceSubTasks(string taskId, [FromBody] List<CreateSubTaskForm> forms, CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                      ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
            {
                return Unauthorized();
            }

            await _toDoTaskService.ReplaceSubTasksAsync(taskId, userId, forms, cancellationToken);
            return NoContent();
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(string term, int page = PaginationConfiguration.MinimumPage, int pageSize = PaginationConfiguration.DefaultPageSize, CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                      ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
            {
                return Unauthorized();
            }
            return Ok(await _toDoTaskService.SearchAsync(term, userId, page, pageSize, cancellationToken));
        }
    }
}
