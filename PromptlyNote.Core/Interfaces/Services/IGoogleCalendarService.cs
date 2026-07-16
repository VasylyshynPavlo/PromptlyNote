using PromptlyNote.Core.DTOs.LightDTOs;

namespace PromptlyNote.Core.Interfaces.Services
{
    public interface IGoogleCalendarService
    {
        Task ConnectAsync(string userId, string code, string redirectUri, CancellationToken cancellationToken = default);
        Task<string> CreateEventAsync(string userId, ToDoTaskLightDto dto, CancellationToken cancellationToken = default);

        Task DeleteEventAsync(string userId, string taskId, CancellationToken cancellationToken = default);
        Task DisconnectAsync(string userId, CancellationToken cancellationToken = default);
        Task<bool> IsConnectedAsync(string userId, CancellationToken cancellationToken = default);
    }
}
