using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PromptlyNote.Core.DTOs.Forms.Update
{
    public class UpdateTaskListForm
    {
        [MaxLength(255, ErrorMessage = "Name cannot exceed 255 characters.")]
        [MinLength(1, ErrorMessage = "Name must be at least 1 character long.")]
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(255, ErrorMessage = "Description cannot exceed 255 characters.")]
        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Icon name is required.")]
        public string IconName { get; set; } = string.Empty;
    }
}
