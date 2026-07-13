using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        private const string FrontendResultUrl = "http://localhost:5173/settings";

        [HttpGet("connect")]
        public IActionResult Connect()
        {
            var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                      ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
            {
                return Unauthorized();
            }

            var url = _googleCalendarService.BuildConnectUrl(userId);
            return Ok(new { url });
        }

        [AllowAnonymous]
        [HttpGet("callback")]
        public async Task<IActionResult> Callback(
            string? code, string? state, string? error, CancellationToken cancellationToken = default)
        {
            if (!string.IsNullOrEmpty(error) || string.IsNullOrEmpty(code) || string.IsNullOrEmpty(state))
            {
                return Redirect($"{FrontendResultUrl}?calendar=error");
            }

            try
            {
                await _googleCalendarService.HandleConnectCallbackAsync(code, state, cancellationToken);
            }
            catch (ApiException)
            {
                return Redirect($"{FrontendResultUrl}?calendar=error");
            }

            return Redirect($"{FrontendResultUrl}?calendar=connected");
        }
    }
}
