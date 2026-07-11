using PromptlyNote.Core.Entities;
using PromptlyNote.Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace PromptlyNote.Data.Repositories
{
    public class RefreshTokenRepository(ApplicationDbContext context) : IRefreshTokenRepository
    {
        private readonly ApplicationDbContext _context = context;

        public async Task AddAsync(RefreshToken token, CancellationToken cancellationToken = default)
        {
            await _context.RefreshTokens.AddAsync(token, cancellationToken);
        }

        public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
        {
            return await _context.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == token, cancellationToken);
        }

        public async Task<bool> TryRevokeAsync(Guid tokenId, string? replacedByToken, CancellationToken cancellationToken = default)
        {
            var updated = await _context.RefreshTokens
                .Where(rt => rt.Id == tokenId && rt.RevokedAt == null)
                .ExecuteUpdateAsync(rt => rt
                    .SetProperty(t => t.RevokedAt, DateTime.UtcNow)
                    .SetProperty(t => t.ReplacedByToken, replacedByToken), cancellationToken);

            return updated > 0;
        }

        public async Task RevokeSessionAsync(Guid sessionId, CancellationToken cancellationToken = default)
        {
            await _context.RefreshTokens
                .Where(rt => rt.SessionId == sessionId && rt.RevokedAt == null)
                .ExecuteUpdateAsync(rt => rt.SetProperty(t => t.RevokedAt, DateTime.UtcNow), cancellationToken);
        }

        public async Task RevokeAllForUserAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            await _context.RefreshTokens
                .Where(rt => rt.UserId == userId && rt.RevokedAt == null)
                .ExecuteUpdateAsync(rt => rt.SetProperty(t => t.RevokedAt, DateTime.UtcNow), cancellationToken);
        }

        public async Task<int> CountForUserAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _context.RefreshTokens.CountAsync(rt => rt.UserId == userId, cancellationToken);
        }

        public async Task DeleteDeadTokensForUserAsync(Guid userId, DateTime revokedBefore, CancellationToken cancellationToken = default)
        {
            await _context.RefreshTokens
                .Where(rt => rt.UserId == userId
                    && (rt.ExpiresAt < DateTime.UtcNow
                        || (rt.RevokedAt != null && rt.RevokedAt < revokedBefore)))
                .ExecuteDeleteAsync(cancellationToken);
        }
    }
}
