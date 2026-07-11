namespace PromptlyNote.Core.Constants
{
    public static class AuthCookieConfiguration
    {
        public const string AccessTokenCookieName = "auth_token";
        public const string RefreshTokenCookieName = "refresh_token";
        public const string UserInfoCookieName = "user_info";

        public const int RefreshTokenLifetimeDays = 30;
    }
}
