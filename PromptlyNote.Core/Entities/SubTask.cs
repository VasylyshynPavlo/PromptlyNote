namespace PromptlyNote.Core.Entities
{
    public class SubTask
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
    }
}
