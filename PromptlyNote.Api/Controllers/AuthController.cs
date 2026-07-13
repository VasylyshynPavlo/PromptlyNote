using Microsoft.AspNetCore.Mvc;
using PromptlyNote.Core.Constants;
using PromptlyNote.Core.DTOs.Forms.Auth;
using PromptlyNote.Core.Interfaces.Services;
using System.Text.Json;

namespace PromptlyNote.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        private readonly IAuthService _authService = authService;

        [HttpPost]
        public async Task<IActionResult> Login([FromForm] LoginForm form, CancellationToken cancellationToken = default)
        {
            var (accessToken, userLightDto) = await _authService.LoginAsync(form, cancellationToken);
            SetAuthCookies(accessToken);
            return Ok(userLightDto);
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromForm] RegisterForm form, CancellationToken cancellationToken = default)
        {
            var (accessToken, userLightDto) = await _authService.RegisterAsync(form, cancellationToken);
            SetAuthCookies(accessToken);
            return Ok(userLightDto);
        }

        [HttpPost]
        public async Task<IActionResult> LoginViaGoogle([FromForm] GoogleAuthForm form, CancellationToken cancellationToken = default)
        {
            var (accessToken, userLightDto) = await _authService.AuthViaGoogleAsync(form.Code, form.RedirectUri, cancellationToken);
            SetAuthCookies(accessToken);
            return Ok(userLightDto);
        }

        [HttpPost]
        public IActionResult Logout()
        {
            ClearAuthCookies();
            return NoContent();
        }

        private void SetAuthCookies(string accessToken)
        {
            var expires = DateTimeOffset.UtcNow.AddDays(AuthCookieConfiguration.TokenLifetimeDays);

            Response.Cookies.Append(
                AuthCookieConfiguration.AccessTokenCookieName,
                accessToken,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Path = "/",
                    Expires = expires
                });

            var userInfo = new { logged_in = true };
            Response.Cookies.Append(
                AuthCookieConfiguration.UserInfoCookieName,
                Uri.EscapeDataString(JsonSerializer.Serialize(userInfo)),
                new CookieOptions
                {
                    HttpOnly = false,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Path = "/",
                    Expires = expires
                });
        }

        private void ClearAuthCookies()
        {
            var expiredHttpOnly = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/",
                Expires = DateTimeOffset.UnixEpoch
            };
            var expiredReadable = new CookieOptions
            {
                HttpOnly = false,
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/",
                Expires = DateTimeOffset.UnixEpoch
            };

            Response.Cookies.Append(AuthCookieConfiguration.AccessTokenCookieName, string.Empty, expiredHttpOnly);
            Response.Cookies.Append(AuthCookieConfiguration.UserInfoCookieName, string.Empty, expiredReadable);
        }
    }
}
