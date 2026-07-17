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
        IGoogleTokenProtector tokenProtector,
        IGoogleCalendarConnectionRepository connectionRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork) : IGoogleCalendarService
    {
        private static readonly string[] ConnectScopes =
            ["openid", "email", CalendarService.Scope.CalendarEvents];
        private static readonly string ConnectScopeString = string.Join(' ', ConnectScopes);

        private readonly string _clientId = configuration["Authentication:Google:ClientId"]
            ?? throw new ArgumentNullException("Google ClientId is not configured.");
        private readonly string _clientSecret = configuration["Authentication:Google:ClientSecret"]
            ?? throw new ArgumentNullException("Google ClientSecret is not configured.");

        private readonly IGoogleTokenProtector _tokenProtector = tokenProtector;
        private readonly IGoogleCalendarConnectionRepository _connectionRepository = connectionRepository;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task ConnectAsync(string userId, string code, string redirectUri, CancellationToken cancellationToken = default)
        {
            var userGuid = userId.ParseToGuidWithThrow("user");

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
                userId: userId,
                code: code,
                redirectUri: redirectUri,
                cancellationToken);

            if (string.IsNullOrEmpty(tokenResponse.RefreshToken))
            {
                throw new InternalException("Google did not return a refresh token.");
            }

            GoogleJsonWebSignature.Payload payload;
            try
            {
                payload = await GoogleJsonWebSignature.ValidateAsync(
                    tokenResponse.IdToken,
                    new GoogleJsonWebSignature.ValidationSettings { Audience = [_clientId] });
            }
            catch (InvalidJwtException ex)
            {
                throw new ForbiddenException("Invalid Google token.", ex);
            }

            if (!payload.EmailVerified)
            {
                throw new ForbiddenException("The Google account email is not verified.");
            }

            var user = await _userRepository.FindAsync(u => u.Id == userGuid, cancellationToken)
                ?? throw new NotFoundException("user");

            if (user.GoogleSub is null)
            {
                if (await _userRepository.ExistsAsync(u => u.GoogleSub == payload.Subject, cancellationToken))
                {
                    throw new ConflictException("This Google account is already linked to another account.");
                }

                user.GoogleSub = payload.Subject;
                await _userRepository.UpdateAsync(user);
            }
            else if (user.GoogleSub != payload.Subject)
            {
                throw new ForbiddenException("Connect the calendar of the Google account you signed in with.");
            }

            var encryptedRefreshToken = _tokenProtector.Protect(tokenResponse.RefreshToken);

            var existing = await _connectionRepository.FindAsync(
                gc => gc.UserId == userGuid, cancellationToken);

            if (existing is null)
            {
                await _connectionRepository.AddAsync(new GoogleCalendarConnection
                {
                    UserId = userGuid,
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

        public async Task<string> CreateEventAsync(string userId, ToDoTaskLightDto dto, CancellationToken cancellationToken = default)
        {
            using var calendar = await BuildCalendarClientAsync(userId, cancellationToken);

            if (dto.DueDate is null)
            {
                throw new BadRequestException("Due date is required to create a Google Calendar event.");
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
                Scopes = [CalendarService.Scope.CalendarEvents]
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
    }
}
