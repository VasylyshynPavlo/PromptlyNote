using PromptlyNote.Core.DTOs.LightDTOs;

namespace PromptlyNote.Core.DTOs
{
    public class UserDto : BaseDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool GoogleCalendar { get; set; } = false;
        public bool IsGoogleLinked { get; init; } = false;
        public bool IsPasswordSet { get; init; } = false;
    }
}
