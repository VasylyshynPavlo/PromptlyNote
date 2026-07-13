using System.ComponentModel.DataAnnotations;

namespace PromptlyNote.Core.DTOs.Forms.Create
{
    public class CreateUserForm
    {
        [Url(ErrorMessage = "Avatar URL must be a valid URL.")]
        public string? AvatarUrl { get; set; }
        [Required(ErrorMessage = "Full name is required.")]
        [MaxLength(255, ErrorMessage = "Full name cannot exceed 255 characters.")]
        public string FullName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Email must be a valid email address.")]
        [MaxLength(254, ErrorMessage = "Email cannot exceed 254 characters.")]
        public string Email { get; set; } = string.Empty;
        [Required(ErrorMessage = "Password is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string? Password { get; set; }
    }
}
