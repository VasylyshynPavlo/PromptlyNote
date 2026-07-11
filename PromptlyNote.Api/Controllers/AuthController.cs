using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PromptlyNote.Core.Constants;
using PromptlyNote.Core.DTOs.Forms.Auth;
using PromptlyNote.Core.DTOs.LightDTOs;
using PromptlyNote.Core.Interfaces.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;

namespace PromptlyNote.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController(IAuthService authService, IConfiguration configuration) : ControllerBase
    {
        private readonly IAuthService _authService = authService;
        private readonly IConfiguration _configuration = configuration;

        [HttpPost]
        public async Task<IActionResult> Login([FromForm] LoginForm form, CancellationToken cancellationToken = default)
        {
            var (accessToken, refreshToken, userLightDto) = await _authService.LoginAsync(form, cancellationToken);
            SetAuthTokenCookies(accessToken, refreshToken);
            SetUserInfoCookie();
            return Ok(userLightDto);
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromForm] RegisterForm form, CancellationToken cancellationToken = default)
        {
            var (accessToken, refreshToken, userLightDto) = await _authService.RegisterAsync(form, cancellationToken);
            SetAuthTokenCookies(accessToken, refreshToken);
            SetUserInfoCookie();
            return Ok(userLightDto);
        }

        [HttpPost]
        public async Task<IActionResult> LoginViaGoogle([FromForm] GoogleAuthForm form, CancellationToken cancellationToken = default)
        {
            var (accessToken, refreshToken, userLightDto) = await _authService.AuthViaGoogleAsync(form.Code, form.RedirectUri, cancellationToken);
            SetAuthTokenCookies(accessToken, refreshToken);
            SetUserInfoCookie();
            return Ok(userLightDto);
        }

        [HttpPost]
        public async Task<IActionResult> Logout(CancellationToken cancellationToken = default)
        {
            if (Request.Cookies.TryGetValue(AuthCookieConfiguration.RefreshTokenCookieName, out var refreshToken))
            {
                await _authService.LogoutAsync(refreshToken, cancellationToken);
            }
            ClearAuthCookies();
            return NoContent();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ConnectGoogle([FromForm] GoogleAuthForm form, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> IsLoggedIn(CancellationToken cancellationToken = default)
        {
            return NoContent();
        }

        //[HttpPost]
        //public async Task<IActionResult> Refresh(CancellationToken cancellationToken = default)
        //{
        //    if (!Request.Cookies.TryGetValue(AuthCookieConfiguration.RefreshTokenCookieName, out var refreshToken))
        //        return Unauthorized();

        //    try
        //    {
        //        var (accessToken, newRefreshToken) = await _authService.RefreshAsync(refreshToken, cancellationToken);
        //        SetAuthTokenCookies(accessToken, newRefreshToken);
        //    }
        //    catch (UnauthorizedAccessException)
        //    {
        //        ClearAuthCookies();
        //        return Unauthorized();
        //    }
        //    if (!Request.Cookies.TryGetValue(AuthCookieConfiguration.UserInfoCookieName, out var userInfoCookie))
        //    {
        //        if (Request.Cookies.TryGetValue(AuthCookieConfiguration.AccessTokenCookieName, out var accessTokenCookie))
        //        {
        //            var handler = new JwtSecurityTokenHandler();
        //            var jwtToken = handler.ReadJwtToken(accessTokenCookie);
        //            var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "sub");
        //            if (userIdClaim == null || string.IsNullOrEmpty(userIdClaim.Value))
        //                return Unauthorized();

        //            SetUserInfoCookie(userIdClaim.Value);
        //        }
        //        return Unauthorized();
        //    }
        //    return NoContent();
        //}

        private void SetAuthTokenCookies(string accessToken, string refreshToken)
        {
            var accessTokenMinutes = int.Parse(_configuration["JwtSettings:AccessTokenMinutes"] ?? "15");

            Response.Cookies.Append(AuthCookieConfiguration.AccessTokenCookieName, accessToken, TokenCookieOptions(DateTimeOffset.UtcNow.AddMinutes(accessTokenMinutes)));
            Response.Cookies.Append(AuthCookieConfiguration.RefreshTokenCookieName, refreshToken, TokenCookieOptions(DateTimeOffset.UtcNow.AddDays(AuthCookieConfiguration.RefreshTokenLifetimeDays)));
        }

        private void SetUserInfoCookie()
        {
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
                    Expires = DateTimeOffset.UtcNow.AddDays(AuthCookieConfiguration.RefreshTokenLifetimeDays)
                });
        }

        private static CookieOptions TokenCookieOptions(DateTimeOffset expires) => new()
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Path = "/",
            Expires = expires
        };

        private void ClearAuthCookies()
        {
            var expiredHttpOnly = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UnixEpoch
            };
            var expiredReadable = new CookieOptions
            {
                HttpOnly = false,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UnixEpoch
            };

            Response.Cookies.Append(AuthCookieConfiguration.AccessTokenCookieName, string.Empty, expiredHttpOnly);
            Response.Cookies.Append(AuthCookieConfiguration.RefreshTokenCookieName, string.Empty, expiredHttpOnly);
            Response.Cookies.Append(AuthCookieConfiguration.UserInfoCookieName, string.Empty, expiredReadable);
        }
    }
}
