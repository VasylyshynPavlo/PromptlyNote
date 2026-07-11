using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PromptlyNote.Core.Constants;
using PromptlyNote.Core.DTOs.Forms.Create;
using PromptlyNote.Core.DTOs.Forms.Update;
using PromptlyNote.Core.Enums;
using PromptlyNote.Core.Interfaces.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace PromptlyNote.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TaskListController(ITaskListService taskListService, IUserService userService) : ControllerBase
    {
        private readonly ITaskListService _taskListService = taskListService;
        private readonly IUserService _userService = userService;

        [HttpGet]
        public async Task<IActionResult> Get(string id, bool includeTasks = false, CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                      ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            return Ok(await _taskListService.GetAsync(id, userId, includeTasks, cancellationToken));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateTaskListForm createTaskListForm, CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                      ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            await _taskListService.CreateAsync(createTaskListForm, userId, cancellationToken);
            return Created();
        }

        [HttpGet]
        public async Task<IActionResult> List(int page = PaginationConfiguration.MinimumPage, int pageSize = PaginationConfiguration.DefaultPageSize, TaskListSortBy sortBy = TaskListSortBy.Name, bool includeTasks = false, CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                      ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            return Ok(await _taskListService.ListAsync(userId, page, pageSize, sortBy, includeTasks, cancellationToken));
        }

        [HttpPut]
        public async Task<IActionResult> Update(string id, [FromForm] UpdateTaskListForm updateTaskListForm, CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                      ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            await _taskListService.UpdateAsync(id, userId, updateTaskListForm, cancellationToken);
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

            await _taskListService.DeleteAsync(id, userId, cancellationToken);
            return Ok();
        }
    }
}
