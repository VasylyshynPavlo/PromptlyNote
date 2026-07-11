using System;
using System.Collections.Generic;
using System.Text;

namespace PromptlyNote.Core.DTOs
{
    public class SubTaskDto : BaseDto
    {
        public string Name { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
    }
}
