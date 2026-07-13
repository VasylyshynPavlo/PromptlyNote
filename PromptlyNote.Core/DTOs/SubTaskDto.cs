namespace PromptlyNote.Core.DTOs
{
    public class SubTaskDto : BaseDto
    {
        public string Name { get; set; } = string.Empty;
        public int Order { get; set; }
        public bool IsCompleted { get; set; }
    }
}
