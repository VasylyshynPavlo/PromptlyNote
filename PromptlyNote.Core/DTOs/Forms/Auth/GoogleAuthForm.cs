using System.ComponentModel.DataAnnotations;

namespace PromptlyNote.Core.DTOs.Forms.Auth
{
    public class GoogleAuthForm
    {
        [Required(ErrorMessage = "Code is required.")]
        public string Code { get; set; } = string.Empty;

        [Required(ErrorMessage = "Redirect URI is required.")]
        [Url(ErrorMessage = "Redirect URI is invalid.")]
        public string RedirectUri { get; set; } = string.Empty;
    }
}
