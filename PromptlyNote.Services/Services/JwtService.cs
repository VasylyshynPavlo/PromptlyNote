using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PromptlyNote.Core.Constants;
using PromptlyNote.Core.Entities;
using PromptlyNote.Core.Exceptions;
using PromptlyNote.Core.Interfaces;
using PromptlyNote.Core.Interfaces.Repositories;
using PromptlyNote.Core.Interfaces.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace PromptlyNote.Services.Services
{
    public class JwtService(
        IConfiguration configuration,
        IRefreshTokenRepository refreshTokenRepository,
        IUnitOfWork unitOfWork) : IJwtService
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        private static readonly TimeSpan RotationGracePeriod = TimeSpan.FromSeconds(30);
        private const int CleanupThreshold = 10;

        public string GenerateAccessToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(JwtRegisteredClaimNames.Email, user.Email),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(int.Parse(jwtSettings["AccessTokenMinutes"] ?? "15")),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static string GenerateRefreshTokenValue()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }

        public async Task<(string AccessToken, string RefreshToken)> LoginAsync(User user, CancellationToken cancellationToken = default)
        {
            var accessToken = GenerateAccessToken(user);
            var refreshTokenValue = GenerateRefreshTokenValue();

            var refreshToken = new RefreshToken
            {
                Token = refreshTokenValue,
                UserId = user.Id,
                SessionId = Guid.NewGuid(),
                ExpiresAt = DateTime.UtcNow.AddDays(AuthCookieConfiguration.RefreshTokenLifetimeDays)
            };

            await _refreshTokenRepository.AddAsync(refreshToken, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await CleanupDeadTokensIfNeededAsync(user.Id, cancellationToken);

            return (accessToken, refreshTokenValue);
        }

        public async Task<(string AccessToken, string RefreshToken)> RefreshAsync(string refreshToken, CancellationToken cancellationToken = default)
        {
            var existingToken = await _refreshTokenRepository.GetByTokenAsync(refreshToken, cancellationToken)
                ?? throw new UnauthorizedAccessException("Invalid refresh token.");

            if (existingToken.User is null || DateTime.UtcNow >= existingToken.ExpiresAt)
                throw new UnauthorizedAccessException("Refresh token expired.");

            if (existingToken.RevokedAt is not null)
                return await HandleRevokedTokenAsync(existingToken, cancellationToken);

            var user = existingToken.User;
            var newRefreshTokenValue = GenerateRefreshTokenValue();

            bool revoked;
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                revoked = await _refreshTokenRepository.TryRevokeAsync(existingToken.Id, newRefreshTokenValue, cancellationToken);

                if (revoked)
                {
                    await _refreshTokenRepository.AddAsync(new RefreshToken
                    {
                        Token = newRefreshTokenValue,
                        UserId = user.Id,
                        SessionId = existingToken.SessionId,
                        ExpiresAt = DateTime.UtcNow.AddDays(AuthCookieConfiguration.RefreshTokenLifetimeDays)
                    }, cancellationToken);
                }

                await _unitOfWork.CommitAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                throw new InternalException("An error occurred while refreshing the token.", ex);
            }

            if (!revoked)
            {
                existingToken = await _refreshTokenRepository.GetByTokenAsync(refreshToken, cancellationToken)
                    ?? throw new UnauthorizedAccessException("Invalid refresh token.");
                return await HandleRevokedTokenAsync(existingToken, cancellationToken);
            }

            await CleanupDeadTokensIfNeededAsync(user.Id, cancellationToken);

            return (GenerateAccessToken(user), newRefreshTokenValue);
        }

        public async Task RevokeAsync(string refreshToken, CancellationToken cancellationToken = default)
        {
            var existingToken = await _refreshTokenRepository.GetByTokenAsync(refreshToken, cancellationToken);
            if (existingToken is not null)
            {
                await _refreshTokenRepository.RevokeSessionAsync(existingToken.SessionId, cancellationToken);
            }
        }

        private async Task<(string AccessToken, string RefreshToken)> HandleRevokedTokenAsync(RefreshToken revokedToken, CancellationToken cancellationToken)
        {
            var isRecentRotation = revokedToken.RevokedAt is not null
                && DateTime.UtcNow - revokedToken.RevokedAt <= RotationGracePeriod;

            if (isRecentRotation && revokedToken.ReplacedByToken is not null && revokedToken.User is not null)
            {
                var replacement = await _refreshTokenRepository.GetByTokenAsync(revokedToken.ReplacedByToken, cancellationToken);
                if (replacement is not null && replacement.IsActive)
                {
                    return (GenerateAccessToken(revokedToken.User), replacement.Token);
                }
            }

            await _refreshTokenRepository.RevokeSessionAsync(revokedToken.SessionId, cancellationToken);
            throw new UnauthorizedAccessException("Refresh token reuse detected.");
        }

        private async Task CleanupDeadTokensIfNeededAsync(Guid userId, CancellationToken cancellationToken)
        {
            var count = await _refreshTokenRepository.CountForUserAsync(userId, cancellationToken);
            if (count < CleanupThreshold)
                return;

            var revokedBefore = DateTime.UtcNow.AddDays(-1);
            await _refreshTokenRepository.DeleteDeadTokensForUserAsync(userId, revokedBefore, cancellationToken);
        }
    }
}
