using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PromptlyNote.Core.DTOs.Forms.Create;
using PromptlyNote.Core.Exceptions;
using PromptlyNote.Core.Interfaces.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace PromptlyNote.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CalendarController(IGoogleCalendarService googleCalendarService) : ControllerBase
    {
        private readonly IGoogleCalendarService _googleCalendarService = googleCalendarService;

        private const string FrontendResultUrl = "http://localhost:5173/settings";

        [HttpGet]
        public IActionResult Connect()
        {
            var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                      ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            var url = _googleCalendarService.BuildConnectUrl(userId);
            return Ok(new { url });
        }

        [AllowAnonymous]
        [HttpGet]
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

        [HttpPost]
        public async Task<IActionResult> CreateEvent([FromBody] CreateCalendarEventForm form, CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                      ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            var eventId = await _googleCalendarService.CreateEventAsync(userId, form, cancellationToken);
            return Ok(new { eventId });
        }

        [HttpGet]
        public async Task<IActionResult> ListEvents(CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                      ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            return Ok(await _googleCalendarService.ListEventsAsync(userId, cancellationToken));
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteEvent(string eventId, CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                      ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            await _googleCalendarService.DeleteEventAsync(userId, eventId, cancellationToken);
            return NoContent();
        }
    }
}
