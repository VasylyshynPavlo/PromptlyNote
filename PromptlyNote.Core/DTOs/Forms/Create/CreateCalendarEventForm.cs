namespace PromptlyNote.Core.DTOs.Forms.Create
{
    public class CreateCalendarEventForm
    {
        public string Summary { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTimeOffset Start { get; set; }
        public DateTimeOffset End { get; set; }
    }
}
