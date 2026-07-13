using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PromptlyNote.Core.Interfaces.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace PromptlyNote.Api.Controllers
{
    [Authorize]
    [Route("api/user")]
    [ApiController]
    public class UserController(IUserService userService) : ControllerBase
    {
        private readonly IUserService _userService = userService;

        [HttpGet]
        public async Task<IActionResult> Me(bool includeCategory = false, bool includeTasks = false, bool includeTaskLists = false, CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                      ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
            {
                return Unauthorized();
            }

            var user = await _userService.GetAsync(userId, includeCategory, includeTasks, includeTaskLists, cancellationToken);
            return user is null ? throw new KeyNotFoundException("User not found") : (IActionResult)Ok(user);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateFullName(string fullName, CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                      ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
            {
                return Unauthorized();
            }

            await _userService.ChangeFullNameAsync(fullName, userId, cancellationToken);
            return NoContent();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteMe(CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                      ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
            {
                return Unauthorized();
            }

            await _userService.DeleteAsync(userId, cancellationToken);
            return NoContent();
        }
    }
}
