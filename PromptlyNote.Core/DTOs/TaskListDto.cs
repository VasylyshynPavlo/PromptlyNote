using PromptlyNote.Core.DTOs.LightDTOs;

namespace PromptlyNote.Core.DTOs
{
    public class TaskListDto : BaseDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string IconName { get; set; } = string.Empty;
        public bool Default { get; set; } = false;
        public string UserId { get; set; } = string.Empty;
        public int TaskCount { get; set; } = 0;
    }
}
