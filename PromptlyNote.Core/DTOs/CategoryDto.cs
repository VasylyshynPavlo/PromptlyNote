using PromptlyNote.Core.DTOs.LightDTOs;
using PromptlyNote.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PromptlyNote.Core.DTOs
{
    public class CategoryDto : BaseDto
    {
        public string Name { get; set; } = string.Empty;
        public string ColorHex { get; set; } = string.Empty;
        public bool Default { get; set; } = false;
        public string UserId { get; set; } = string.Empty;
        public List<ToDoTaskLightDto> Tasks { get; set; } = [];
    }
}
