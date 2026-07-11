namespace PromptlyNote.Core.Entities
{
    public class ToDoTask : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;
        public DateTime? DueDate { get; set; }
        public bool IsCompleted { get; set; } = false;
        public Guid? CategoryId { get; set; }
        public Category? Category { get; set; }

        public Guid UserId { get; set; }
        public User? User { get; set; }

        public Guid TaskListId { get; set; }
        public TaskList? TaskList { get; set; }
        public List<SubTask> SubTasks { get; set; } = [];
    }
}
