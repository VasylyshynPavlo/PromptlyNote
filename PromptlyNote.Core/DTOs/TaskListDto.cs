using PromptlyNote.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PromptlyNote.Core.DTOs
{
    public class TaskListDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string IconName { get; set; } = string.Empty;
        public bool Default { get; set; } = false;
        public string UserId { get; set; } = string.Empty;
        public UserDto? User { get; set; }
        public List<ToDoTaskDto> Tasks { get; set; } = [];
    }
}
