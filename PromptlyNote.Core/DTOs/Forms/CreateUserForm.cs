using PromptlyNote.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PromptlyNote.Core.DTOs.Forms
{
    public class CreateUserForm
    {
        public string? AvatarUrl { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Password { get; set; }
    }
}
