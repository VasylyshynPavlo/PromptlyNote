using PromptlyNote.Core.DTOs.LightDTOs;

namespace PromptlyNote.Core.DTOs
{
    public class UserDto : BaseDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool GoogleAuth { get; set; } = false;
        public List<ToDoTaskLightDto> Tasks { get; set; } = [];
        public List<CategoryLightDto> Categories { get; set; } = [];
        public List<TaskListLightDto> TaskLists { get; set; } = [];
    }
}
