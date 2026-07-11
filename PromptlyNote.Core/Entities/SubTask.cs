namespace PromptlyNote.Core.Entities
{
    public class SubTask : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
    }
}
