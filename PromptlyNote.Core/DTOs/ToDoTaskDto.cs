using PromptlyNote.Core.DTOs.LightDTOs;

namespace PromptlyNote.Core.DTOs
{
    public class ToDoTaskDto : BaseDto
    {
        public string Name { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;
        public DateTime? DueDate { get; set; }
        public bool IsCompleted { get; set; } = false;
        public string CategoryId { get; set; } = string.Empty;
        public CategoryLightDto? Category { get; set; }

        public string UserId { get; set; } = string.Empty;

        public string TaskListId { get; set; } = string.Empty;
        public TaskListLightDto? TaskList { get; set; }
        public List<SubTaskDto> SubTasks { get; set; } = [];

        public int? RemindBeforeMinutes { get; set; }

        // Намір користувача синхронізувати таску з календарем.
        public bool SyncToGoogleCalendar { get; set; } = false;

        // Чи реально зараз має бути подія в календарі (завершені туди не потрапляють).
        // Саме це поле фронт має показувати як "в календарі".
        public bool IsInGoogleCalendar => SyncToGoogleCalendar && !IsCompleted;
    }
}
