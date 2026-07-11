using PromptlyNote.Core.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PromptlyNote.Core.DTOs.Forms.Create
{
    public class CreateCategoryForm
    {
        [Required(ErrorMessage = "Category name is required.")]
        [MaxLength(50, ErrorMessage = "Category name cannot exceed 50 characters.")]
        [MinLength(1, ErrorMessage = "Category name must be at least 1 character long.")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Category color is required.")]
        [RegularExpression("^#([A-Fa-f0-9]{6})$", ErrorMessage = "Color must be a valid hex code.")]
        public string ColorHex { get; set; } = null!;
    }
}
