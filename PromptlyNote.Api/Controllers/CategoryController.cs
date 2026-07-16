using Microsoft.AspNetCore.Authorization;
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
    [Route("api/category")]
    [ApiController]
    public class CategoryController(ICategoryService categoryService, IUserService userService) : ControllerBase
    {
        private readonly ICategoryService _categoryService = categoryService;
        private readonly IUserService _userService = userService;

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id, CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                      ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
            {
                return Unauthorized();
            }

            return Ok(await _categoryService.GetAsync(id, userId, cancellationToken));
        }

        [HttpGet]
        public async Task<IActionResult> List(int page = PaginationConfiguration.MinimumPage, int pageSize = PaginationConfiguration.DefaultPageSize, CategorySortBy sortBy = CategorySortBy.Name, bool isDescending = false, CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                       ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
            {
                return Unauthorized();
            }

            return Ok(await _categoryService.ListAsync(userId, page, pageSize, sortBy, isDescending, cancellationToken));
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

        [HttpDelete("{id}")]
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

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromForm] UpdateCategoryForm form, CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                      ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
            {
                return Unauthorized();
            }

            await _categoryService.UpdateAsync(id, userId, form, cancellationToken);
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

            return Ok(await _categoryService.SearchAsync(term, userId, page, pageSize, cancellationToken));
        }
    }
}
