namespace PromptlyNote.Core.Entities
{
    public class User : BaseEntity
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PasswordHash { get; set; } = null;
        public List<ToDoTask> Tasks { get; set; } = [];
        public List<Category> Categories { get; set; } = [];
        public List<TaskList> TaskLists { get; set; } = [];
        public GoogleCalendarConnection? GoogleCalendarConnection { get; set; } = null;
    }
}
