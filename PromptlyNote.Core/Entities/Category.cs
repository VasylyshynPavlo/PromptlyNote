namespace PromptlyNote.Core.Entities
{
    public class Category : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string ColorHex { get; set; } = string.Empty;
        public bool Default { get; set; } = false;
        public Guid UserId { get; set; }
        public User? User { get; set; }
        public List<ToDoTask> Tasks { get; set; } = [];

        public Category()
        {
        }

        public Category(string name, string colorHex, Guid userId)
        {
            Name = name;
            ColorHex = colorHex;
            UserId = userId;
        }
    }
}
