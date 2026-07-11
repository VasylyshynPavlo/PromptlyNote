using PromptlyNote.Core.DTOs;
using PromptlyNote.Core.DTOs.Forms.Create;

namespace PromptlyNote.Core.Interfaces.Services
{
    public interface IGoogleCalendarService
    {
        string BuildConnectUrl(string userId);

        Task HandleConnectCallbackAsync(string code, string state, CancellationToken cancellationToken = default);

        Task<string> CreateEventAsync(string userId, CreateCalendarEventForm form, CancellationToken cancellationToken = default);

        Task<List<CalendarEventDto>> ListEventsAsync(string userId, CancellationToken cancellationToken = default);

        Task DeleteEventAsync(string userId, string eventId, CancellationToken cancellationToken = default);
    }
}
