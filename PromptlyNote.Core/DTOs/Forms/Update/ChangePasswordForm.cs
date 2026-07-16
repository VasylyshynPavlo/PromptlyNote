using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PromptlyNote.Core.DTOs.Forms.Update
{
    public class ChangePasswordForm
    {
        [Required(ErrorMessage = "Current password is required.")]
        public string CurrentPassword { get; set; } = null!;

        [Required(ErrorMessage = "New password is required.")]
        [MinLength(6, ErrorMessage = "New password must be at least 6 characters long.")]
        public string NewPassword { get; set; } = null!;
    }
}
