using System;
using System.Collections.Generic;
using System.Text;

namespace PromptlyNote.Core.DTOs.Forms.Create
{
    public class CreateSubTaskForm
    {
        public string Name { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
    }
}
