using PromptlyNote.Core.DTOs;
using PromptlyNote.Core.DTOs.Forms.Auth;
using PromptlyNote.Core.DTOs.LightDTOs;

namespace PromptlyNote.Core.Interfaces.Services
{
    public interface IAuthService
    {
        Task<(string accessToken, UserDto userDto)> RegisterAsync(RegisterForm registerForm, CancellationToken cancellationToken = default);
        Task<(string accessToken, UserDto userDto)> LoginAsync(LoginForm loginForm, CancellationToken cancellationToken = default);
        Task<(string accessToken, UserDto userDto)> AuthViaGoogleAsync(string code, string redirectUri, CancellationToken cancellationToken = default);
    }
}
