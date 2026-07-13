using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PromptlyNote.Core.Constants;
using PromptlyNote.Core.DTOs.Forms.Create;
using PromptlyNote.Core.Enums;
using PromptlyNote.Core.Interfaces.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace PromptlyNote.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CategoryController(ICategoryService categoryService, IUserService userService) : ControllerBase
    {
        private readonly ICategoryService _categoryService = categoryService;
        private readonly IUserService _userService = userService;

        [HttpGet]
        public async Task<IActionResult> Get(string id, bool includeTasks = false, CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                      ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
            {
                return Unauthorized();
            }

            return Ok(await _categoryService.GetAsync(id, userId, includeTasks, cancellationToken));
        }

        [HttpGet]
        public async Task<IActionResult> List(int page = PaginationConfiguration.MinimumPage, int pageSize = PaginationConfiguration.DefaultPageSize, CategorySortBy sortBy = CategorySortBy.Name, bool includeTasks = false, CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                       ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
            {
                return Unauthorized();
            }

            return Ok(await _categoryService.ListAsync(userId, page, pageSize, sortBy, includeTasks, cancellationToken));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateCategoryForm form, CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                      ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
            {
                return Unauthorized();
            }

            await _categoryService.CreateAsync(form, userId, cancellationToken);
            return Created();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                      ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
            {
                return Unauthorized();
            }

            await _categoryService.DeleteAsync(id, userId, cancellationToken);
            return NoContent();
        }
    }
}
