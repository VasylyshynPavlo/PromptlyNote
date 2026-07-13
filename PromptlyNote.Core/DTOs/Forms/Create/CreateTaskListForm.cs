using System.ComponentModel.DataAnnotations;

namespace PromptlyNote.Core.DTOs.Forms.Create
{
    public class CreateTaskListForm
    {
        [Required(ErrorMessage = "Name is required.")]
        [MaxLength(255, ErrorMessage = "Name cannot exceed 255 characters.")]
        [MinLength(1, ErrorMessage = "Name must be at least 1 character long.")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(255, ErrorMessage = "Description cannot exceed 255 characters.")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "IconName is required.")]
        public string IconName { get; set; } = string.Empty;
    }
}
