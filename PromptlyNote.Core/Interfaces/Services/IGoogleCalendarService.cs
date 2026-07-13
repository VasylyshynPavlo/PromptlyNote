using PromptlyNote.Core.DTOs.LightDTOs;

namespace PromptlyNote.Core.Interfaces.Services
{
    public interface IGoogleCalendarService
    {
        string BuildConnectUrl(string userId);

        Task HandleConnectCallbackAsync(string code, string state, CancellationToken cancellationToken = default);

        Task<string> CreateEventAsync(string userId, ToDoTaskLightDto dto, CancellationToken cancellationToken = default);

        Task DeleteEventAsync(string userId, string taskId, CancellationToken cancellationToken = default);
    }
}
