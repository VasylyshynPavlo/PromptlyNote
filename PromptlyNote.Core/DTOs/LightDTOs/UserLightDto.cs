using PromptlyNote.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PromptlyNote.Core.DTOs.LightDTOs
{
    public class UserLightDto : BaseDto
    {
        public string? AvatarUrl { get; set; } = null;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool GoogleAuth { get; set; } = false;
    }
}
