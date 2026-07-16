using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PromptlyNote.Core.DTOs.Forms.Auth;
using PromptlyNote.Core.Exceptions;
using PromptlyNote.Core.Interfaces.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace PromptlyNote.Api.Controllers
{
    [Authorize]
    [Route("api/calendar")]
    [ApiController]
    public class CalendarController(IGoogleCalendarService googleCalendarService) : ControllerBase
    {
        private readonly IGoogleCalendarService _googleCalendarService = googleCalendarService;

        [HttpPut("connect")]
        public async Task<IActionResult> ConnectAsync([FromForm] GoogleForm googleAuthForm, CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                      ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
            {
                return Unauthorized();
            }

            await _googleCalendarService.ConnectAsync(userId, googleAuthForm.Code, googleAuthForm.RedirectUri, cancellationToken);
            return NoContent();
        }
    }
}
