using PromptlyNote.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PromptlyNote.Core.Interfaces.Services
{
    public interface IJwtService
    {
        string GenerateAccessToken(User user);

        Task<(string AccessToken, string RefreshToken)> LoginAsync(User user, CancellationToken cancellationToken = default);
        Task<(string AccessToken, string RefreshToken)> RefreshAsync(string refreshToken, CancellationToken cancellationToken = default);
        Task RevokeAsync(string refreshToken, CancellationToken cancellationToken = default);
    }
}
