namespace PromptlyNote.Core.DTOs.LightDTOs
{
    public class UserLightDto : BaseDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool GoogleAuth { get; set; } = false;
    }
}
