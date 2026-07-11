using PromptlyNote.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PromptlyNote.Core.Interfaces.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task AddAsync(RefreshToken token, CancellationToken cancellationToken = default);
        Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
        Task<bool> TryRevokeAsync(Guid tokenId, string? replacedByToken, CancellationToken cancellationToken = default);
        Task RevokeSessionAsync(Guid sessionId, CancellationToken cancellationToken = default);
        Task RevokeAllForUserAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<int> CountForUserAsync(Guid userId, CancellationToken cancellationToken = default);
        Task DeleteDeadTokensForUserAsync(Guid userId, DateTime revokedBefore, CancellationToken cancellationToken = default);
    }
}