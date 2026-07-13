namespace PromptlyNote.Core.DTOs
{
    public class SubTaskDto : BaseDto
    {
        public string Name { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
    }
}
