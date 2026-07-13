using PromptlyNote.Core.DTOs.Forms.Auth;
using PromptlyNote.Core.DTOs.LightDTOs;

namespace PromptlyNote.Core.Interfaces.Services
{
    public interface IAuthService
    {
        Task<(string accessToken, UserLightDto userLightDto)> RegisterAsync(RegisterForm registerForm, CancellationToken cancellationToken = default);
        Task<(string accessToken, UserLightDto userLightDto)> LoginAsync(LoginForm loginForm, CancellationToken cancellationToken = default);
        Task<(string accessToken, UserLightDto userLightDto)> AuthViaGoogleAsync(string code, string redirectUri, CancellationToken cancellationToken = default);
    }
}
