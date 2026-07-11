namespace PromptlyNote.Core.DTOs
{
    public class CalendarEventDto
    {
        public string Id { get; set; } = string.Empty;
        public string? Summary { get; set; }
        public string? Description { get; set; }
        public DateTimeOffset? Start { get; set; }
        public DateTimeOffset? End { get; set; }
    }
}
