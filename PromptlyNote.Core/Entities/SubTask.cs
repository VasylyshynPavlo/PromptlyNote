namespace PromptlyNote.Core.Entities
{
    public class SubTask : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public int Order { get; set; }
        public bool IsCompleted { get; set; }
    }
}
