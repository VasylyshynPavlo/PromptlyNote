using PromptlyNote.Core.DTOs;
using PromptlyNote.Core.DTOs.Forms.Auth;
using PromptlyNote.Core.DTOs.LightDTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace PromptlyNote.Core.Interfaces.Services
{
    public interface IAuthService
    {
        Task<(string accessToken, string refreshToken, UserLightDto userLightDto)> RegisterAsync(RegisterForm registerForm, CancellationToken cancellationToken = default);
        Task<(string accessToken, string refreshToken, UserLightDto userLightDto)> LoginAsync(LoginForm loginForm, CancellationToken cancellationToken = default);
        Task<(string accessToken, string refreshToken, UserLightDto userLightDto)> AuthViaGoogleAsync(string code, string redirectUri, CancellationToken cancellationToken = default);

        Task<(string accessToken, string refreshToken)> RefreshAsync(string refreshToken, CancellationToken cancellationToken = default);
        Task LogoutAsync(string refreshToken, CancellationToken cancellationToken = default);
    }
}
