using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PromptlyNote.Core.DTOs.Forms.Update
{
    public class SetPasswordForm
    {
        [Required(ErrorMessage = "New password is required.")]
        [MinLength(6, ErrorMessage = "New password must be at least 6 characters long.")]
        public string NewPassword { get; set; } = null!;

        [Required(ErrorMessage = "Code is required.")]
        public string Code { get; set; } = null!;
    }
}
