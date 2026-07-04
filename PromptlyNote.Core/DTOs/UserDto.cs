using PromptlyNote.Core.Entities;

namespace PromptlyNote.Core.DTOs
{
    public class UserDto
    {
        public string Id { get; set; } = string.Empty;
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
