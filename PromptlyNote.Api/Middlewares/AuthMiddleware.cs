using PromptlyNote.Core.Constants;
using PromptlyNote.Core.Interfaces.Services;
using System.Text.Json;

namespace PromptlyNote.Api.Middlewares
{
    public class AuthMiddleware
    {
        // Ендпоінти автентифікації самі керують cookie — їх middleware не чіпає.
        private const string AuthEndpointsPrefix = "/api/auth";

        private readonly RequestDelegate _next;
        private readonly int _accessTokenMinutes;

        public AuthMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _accessTokenMinutes = int.Parse(configuration["JwtSettings:AccessTokenMinutes"] ?? "15");
        }

        // IJwtService — scoped, тому інжектимо його в InvokeAsync (per-request), а не в конструктор.
        public async Task InvokeAsync(HttpContext context, IJwtService jwtService)
        {
            if (context.Request.Path.StartsWithSegments(AuthEndpointsPrefix, StringComparison.OrdinalIgnoreCase))
            {
                await _next(context);
                return;
            }

            var cookies = context.Request.Cookies;
            var hasAccessToken = cookies.ContainsKey(AuthCookieConfiguration.AccessTokenCookieName);
            var isLoggedIn = hasAccessToken;

            // Access token протух (cookie зникла), але refresh ще живий — тихо видаємо новий access.
            if (!hasAccessToken
                && cookies.TryGetValue(AuthCookieConfiguration.RefreshTokenCookieName, out var refreshToken)
                && !string.IsNullOrEmpty(refreshToken))
            {
                try
                {
                    var (newAccessToken, newRefreshToken) =
                        await jwtService.RefreshAsync(refreshToken, context.RequestAborted);

                    SetAuthTokenCookies(context, newAccessToken, newRefreshToken);

                    // Щоб автентифікація спрацювала вже в ЦЬОМУ запиті — підсуваємо свіжий токен.
                    // JwtBearer читає його з заголовка Authorization, бо cookie в запиті ще старе (відсутнє).
                    context.Request.Headers.Authorization = $"Bearer {newAccessToken}";

                    isLoggedIn = true;
                }
                catch (UnauthorizedAccessException)
                {
                    // Refresh недійсний або вже відкликаний (напр. паралельним запитом) —
                    // навмисно НЕ чистимо cookie тут, щоб гонитва не розлогінювала юзера.
                    // Захищені ендпоінти повернуть 401, фронт сам вирішить, що робити.
                    isLoggedIn = false;
                }
            }

            // Мітка для фронта: чи є жива сесія. Якщо мітки нема, а сесія жива — ставимо.
            if (isLoggedIn && !cookies.ContainsKey(AuthCookieConfiguration.UserInfoCookieName))
            {
                SetUserInfoCookie(context);
            }

            await _next(context);
        }

        private void SetAuthTokenCookies(HttpContext context, string accessToken, string refreshToken)
        {
            context.Response.Cookies.Append(
                AuthCookieConfiguration.AccessTokenCookieName,
                accessToken,
                TokenCookieOptions(DateTimeOffset.UtcNow.AddMinutes(_accessTokenMinutes)));

            context.Response.Cookies.Append(
                AuthCookieConfiguration.RefreshTokenCookieName,
                refreshToken,
                TokenCookieOptions(DateTimeOffset.UtcNow.AddDays(AuthCookieConfiguration.RefreshTokenLifetimeDays)));
        }

        private static void SetUserInfoCookie(HttpContext context)
        {
            var userInfo = new { logged_in = true };
            context.Response.Cookies.Append(
                AuthCookieConfiguration.UserInfoCookieName,
                Uri.EscapeDataString(JsonSerializer.Serialize(userInfo)),
                new CookieOptions
                {
                    HttpOnly = false, // читабельна фронтом
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
    }
}
