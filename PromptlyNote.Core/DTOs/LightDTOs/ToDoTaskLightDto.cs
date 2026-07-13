using PromptlyNote.Core.Entities;

namespace PromptlyNote.Core.DTOs.LightDTOs
{
    public class ToDoTaskLightDto : BaseDto
    {
        public string Name { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;
        public DateTime? DueDate { get; set; }
        public bool IsCompleted { get; set; } = false;
        public string CategoryId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string TaskListId { get; set; } = string.Empty;
        public List<SubTask> SubTasks { get; set; } = [];

        public int? RemindBeforeMinutes { get; set; }
        public bool SyncToGoogleCalendar { get; set; } = false;
    }
}
