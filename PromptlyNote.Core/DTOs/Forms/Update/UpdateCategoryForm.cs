using System.ComponentModel.DataAnnotations;

namespace PromptlyNote.Core.DTOs.Forms.Update
{
    public class UpdateCategoryForm
    {
        [MaxLength(50, ErrorMessage = "Category name cannot exceed 50 characters.")]
        [MinLength(1, ErrorMessage = "Category name must be at least 1 character long.")]
        [Required(ErrorMessage = "Category name is required.")]
        public string Name { get; set; } = string.Empty;

        [RegularExpression("^#([A-Fa-f0-9]{6})$", ErrorMessage = "Color must be a valid hex code.")]
        [Required(ErrorMessage = "Color is required.")]
        public string ColorHex { get; set; } = string.Empty;
    }
}
