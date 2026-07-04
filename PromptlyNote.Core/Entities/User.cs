namespace PromptlyNote.Core.Entities
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string? AvatarUrl { get; set; } = null;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PasswordHash { get; set; } = null;
        public bool GoogleAuth { get; set; } = false;
        public List<ToDoTask> Tasks { get; set; } = [];
        public List<Category> Categories { get; set; } = [];
        public List<TaskList> TaskLists { get; set; } = [];
    }
}
