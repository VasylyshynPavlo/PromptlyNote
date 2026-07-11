using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using PromptlyNote.Core.Constants;
using PromptlyNote.Core.DTOs.Forms.Create;
using PromptlyNote.Core.DTOs.Forms.Update;
using PromptlyNote.Core.Entities;
using PromptlyNote.Core.Enums;
using PromptlyNote.Core.Interfaces.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace PromptlyNote.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TaskController(IToDoTaskService toDoTaskService, IUserService userService) : ControllerBase
    {
        private readonly IToDoTaskService _toDoTaskService = toDoTaskService;
        private readonly IUserService _userService = userService;

        [HttpGet]
        public async Task<IActionResult> Get(string id, bool includeCategory = false, bool includeTaskList = false, bool includeSubTasks = false, CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                      ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            return Ok(await _toDoTaskService.GetAsync(id, userId, includeCategory, includeTaskList, includeSubTasks, cancellationToken));
        }

        [HttpGet]
        public async Task<IActionResult> List(int page = PaginationConfiguration.MinimumPage, int pageSize = PaginationConfiguration.DefaultPageSize, ToDoTaskSortBy toDoTaskSortBy = ToDoTaskSortBy.Name, bool includeCategory = false, bool includeTaskList = false, bool includeSubTasks = false, CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                      ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            return Ok(await _toDoTaskService.ListAsync(userId, page, pageSize, toDoTaskSortBy, includeCategory, includeTaskList, includeSubTasks, cancellationToken));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateToDoTaskForm form, CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                      ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            await _toDoTaskService.CreateAsync(form, userId, cancellationToken);
            return Created();
        }

        [HttpPut]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateToDoTaskForm form, CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                      ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            await _toDoTaskService.UpdateAsync(id, userId, form, cancellationToken);
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                      ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            await _toDoTaskService.DeleteAsync(id, userId, cancellationToken);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> AddSubTask(string taskId, [FromForm] CreateSubTaskForm form, CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                      ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            await _toDoTaskService.AddSubTaskAsync(taskId, userId, form, cancellationToken);
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteSubTask(string subTaskId, string taskId, CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                      ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            await _toDoTaskService.DeleteSubTaskAsync(subTaskId, taskId, userId, cancellationToken);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateSubTask(string taskId, [FromForm] UpdateSubTaskForm form, CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                      ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            await _toDoTaskService.UpdateSubTaskAsync(taskId, userId, form, cancellationToken);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> ReplaceSubTasks(string taskId, [FromBody] List<CreateSubTaskForm> forms, CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                      ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            await _toDoTaskService.ReplaceSubTasksAsync(taskId, userId, forms, cancellationToken);
            return Ok();
        }
    }
}
