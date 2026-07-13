using PromptlyNote.Core.DTOs.LightDTOs;

namespace PromptlyNote.Core.DTOs
{
    public class CategoryDto : BaseDto
    {
        public string Name { get; set; } = string.Empty;
        public string ColorHex { get; set; } = string.Empty;
        public bool Default { get; set; } = false;
        public string UserId { get; set; } = string.Empty;
        public List<ToDoTaskLightDto> Tasks { get; set; } = [];
    }
}
