using PromptlyNote.Core.DTOs.LightDTOs;
using PromptlyNote.Core.Entities;

namespace PromptlyNote.Core.DTOs
{
    public class UserDto : BaseDto
    {
        public string? AvatarUrl { get; set; } = null;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool GoogleAuth { get; set; } = false;
        public List<ToDoTaskLightDto> Tasks { get; set; } = [];
        public List<CategoryLightDto> Categories { get; set; } = [];
        public List<TaskListLightDto> TaskLists { get; set; } = [];
    }
}
