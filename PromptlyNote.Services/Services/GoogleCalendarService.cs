using Google;
using Google.Apis.Auth;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Requests;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using PromptlyNote.Core.DTOs.LightDTOs;
using PromptlyNote.Core.Entities;
using PromptlyNote.Core.Exceptions;
using PromptlyNote.Core.Interfaces;
using PromptlyNote.Core.Interfaces.Repositories;
using PromptlyNote.Core.Interfaces.Services;
using PromptlyNote.Core.Utils;

namespace PromptlyNote.Services.Services
{
    public class GoogleCalendarService(
        IConfiguration configuration,
        IDataProtectionProvider dataProtectionProvider,
        IGoogleTokenProtector tokenProtector,
        IGoogleCalendarConnectionRepository connectionRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork) : IGoogleCalendarService
    {
        private static readonly string CalendarScope = CalendarService.Scope.CalendarEvents;

        private static readonly string[] ConnectScopes =
            ["openid", "email", "profile", CalendarService.Scope.CalendarEvents];
        private static readonly string ConnectScopeString = string.Join(' ', ConnectScopes);

        private static readonly TimeSpan StateLifetime = TimeSpan.FromMinutes(10);

        private readonly string _clientId = configuration["Authentication:Google:ClientId"]
            ?? throw new ArgumentNullException("Google ClientId is not configured.");
        private readonly string _clientSecret = configuration["Authentication:Google:ClientSecret"]
            ?? throw new ArgumentNullException("Google ClientSecret is not configured.");
        private readonly string _calendarRedirectUri = configuration["Authentication:Google:CalendarRedirectUri"]
            ?? throw new ArgumentNullException("Google CalendarRedirectUri is not configured.");

        private readonly IDataProtector _stateProtector =
            dataProtectionProvider.CreateProtector("PromptlyNote.GoogleCalendar.State");

        private readonly IGoogleTokenProtector _tokenProtector = tokenProtector;
        private readonly IGoogleCalendarConnectionRepository _connectionRepository = connectionRepository;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public string BuildConnectUrl(string userId)
        {
            var state = _stateProtector.Protect($"{userId}:{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}");

            var request = new GoogleAuthorizationCodeRequestUrl(
                new Uri("https://accounts.google.com/o/oauth2/v2/auth"))
            {
                ClientId = _clientId,
                RedirectUri = _calendarRedirectUri,
                Scope = ConnectScopeString,
                AccessType = "offline",
                Prompt = "consent",
                ResponseType = "code",
                State = state
            };

            return request.Build().ToString();
        }

        public async Task HandleConnectCallbackAsync(string code, string state, CancellationToken cancellationToken = default)
        {
            var userId = ValidateStateAndGetUserId(state);

            using var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new ClientSecrets
                {
                    ClientId = _clientId,
                    ClientSecret = _clientSecret
                },
                Scopes = ConnectScopes
            });

            var tokenResponse = await flow.ExchangeCodeForTokenAsync(
                userId: userId.ToString(),
                code: code,
                redirectUri: _calendarRedirectUri,
                cancellationToken);

            if (string.IsNullOrEmpty(tokenResponse.RefreshToken))
            {
                throw new InternalException("Google did not return a refresh token.");
            }

            var googleEmail = await GetVerifiedGoogleEmailAsync(tokenResponse.IdToken);

            var user = await _userRepository.FindAsync(u => u.Id == userId, cancellationToken)
                ?? throw new NotFoundException("user");

            if (!string.Equals(user.Email, googleEmail, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("The Google account does not match your account. Connect the calendar of the account you are signed in with.");
            }

            var encryptedRefreshToken = _tokenProtector.Protect(tokenResponse.RefreshToken);

            var existing = await _connectionRepository.FindAsync(
                gc => gc.UserId == userId, cancellationToken);

            if (existing is null)
            {
                await _connectionRepository.AddAsync(new GoogleCalendarConnection
                {
                    UserId = userId,
                    EncryptedRefreshToken = encryptedRefreshToken,
                    Scopes = ConnectScopeString
                }, cancellationToken);
            }
            else
            {
                existing.EncryptedRefreshToken = encryptedRefreshToken;
                existing.Scopes = ConnectScopeString;
                await _connectionRepository.UpdateAsync(existing);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        private async Task<string> GetVerifiedGoogleEmailAsync(string? idToken)
        {
            if (string.IsNullOrEmpty(idToken))
            {
                throw new InternalException("Google did not return an id token.");
            }

            GoogleJsonWebSignature.Payload payload;
            try
            {
                payload = await GoogleJsonWebSignature.ValidateAsync(
                    idToken,
                    new GoogleJsonWebSignature.ValidationSettings { Audience = [_clientId] });
            }
            catch (InvalidJwtException ex)
            {
                throw new ArgumentException("Invalid Google token.", ex);
            }

            if (!payload.EmailVerified)
            {
                throw new ArgumentException("Google email is not verified.");
            }

            return payload.Email;
        }

        public async Task<string> CreateEventAsync(string userId, ToDoTaskLightDto dto, CancellationToken cancellationToken = default)
        {
            using var calendar = await BuildCalendarClientAsync(userId, cancellationToken);

            if (dto.DueDate is null)
            {
                throw new ArgumentException("Due date is required to create a Google Calendar event.");
            }

            var googleEvent = new Event
            {
                Summary = $"Task completion deadline: {dto.Name}",
                Description = dto.Note,
                Start = new EventDateTime { DateTimeDateTimeOffset = dto.DueDate },
                End = new EventDateTime { DateTimeDateTimeOffset = dto.DueDate + TimeSpan.FromHours(1) },
                ExtendedProperties = new Event.ExtendedPropertiesData
                {
                    Private__ = new Dictionary<string, string>
                    {
                        { "source", "PromptlyNote" },
                        { "taskId", dto.Id.ToLower() }
                    }
                }
            };

            if (dto.RemindBeforeMinutes is not null)
            {
                googleEvent.Reminders = new Event.RemindersData
                {
                    UseDefault = false,
                    Overrides = [new EventReminder
                    {
                        Method = "popup",
                        Minutes = dto.RemindBeforeMinutes.Value
                    }]
                };
            }

            var created = await calendar.Events.Insert(googleEvent, "primary").ExecuteAsync(cancellationToken);
            return created.Id;
        }

        public async Task DeleteEventAsync(string userId, string taskId, CancellationToken cancellationToken = default)
        {
            using var calendar = await BuildCalendarClientAsync(userId, cancellationToken);

            var request = calendar.Events.List("primary");
            request.PrivateExtendedProperty = new Repeatable<string>([$"taskId={taskId.ToLower()}", "source=PromptlyNote"]);

            var events = await request.ExecuteAsync(cancellationToken);

            var eventToDelete = events.Items.FirstOrDefault();

            if (eventToDelete is null)
                return;

            try
            {
                await calendar.Events.Delete("primary", eventToDelete.Id).ExecuteAsync(cancellationToken);

            }
            catch (GoogleApiException ex)
            {
                throw new InternalException("Failed to delete Google Calendar event.", ex);
            }
        }

        private async Task<CalendarService> BuildCalendarClientAsync(string userId, CancellationToken cancellationToken)
        {
            var userGuid = userId.ParseToGuidWithThrow("user");

            var connection = await _connectionRepository.FindAsync(gc => gc.UserId == userGuid, cancellationToken)
                ?? throw new NotFoundException("google calendar connection");

            var refreshToken = _tokenProtector.Unprotect(connection.EncryptedRefreshToken);

            var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new ClientSecrets { ClientId = _clientId, ClientSecret = _clientSecret },
                Scopes = [CalendarScope]
            });
            var credential = new UserCredential(flow, userGuid.ToString(), new TokenResponse { RefreshToken = refreshToken });

            return new CalendarService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "PromptlyNote"
            });
        }

        public async Task DisconnectAsync(string userId, CancellationToken cancellationToken = default)
        {
            var userGuid = userId.ParseToGuidWithThrow("user");
            var connection = await _connectionRepository.FindAsync(gc => gc.UserId == userGuid, cancellationToken);
            if (connection is null)
            {
                return;
            }
            await _connectionRepository.DeleteAsync(connection.Id, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> IsConnectedAsync(string userId, CancellationToken cancellationToken = default)
        {
            var userGuid = userId.ParseToGuidWithThrow("user");
            var connection = await _connectionRepository.FindAsync(gc => gc.UserId == userGuid, cancellationToken);
            return connection is not null;
        }

        private Guid ValidateStateAndGetUserId(string state)
        {
            string decoded;
            try
            {
                decoded = _stateProtector.Unprotect(state);
            }
            catch
            {
                throw new ArgumentException("Invalid OAuth state.");
            }

            var parts = decoded.Split(':');
            if (parts.Length != 2
                || !Guid.TryParse(parts[0], out var userId)
                || !long.TryParse(parts[1], out var issuedAtUnix))
            {
                throw new ArgumentException("Invalid OAuth state.");
            }

            var issuedAt = DateTimeOffset.FromUnixTimeSeconds(issuedAtUnix);
            if (DateTimeOffset.UtcNow - issuedAt > StateLifetime)
            {
                throw new ArgumentException("OAuth state has expired.");
            }

            return userId;
        }
    }
}
