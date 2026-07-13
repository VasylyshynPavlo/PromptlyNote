namespace PromptlyNote.Core.DTOs.Forms.Update
{
    public class UpdateSubTaskForm
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
    }
}
