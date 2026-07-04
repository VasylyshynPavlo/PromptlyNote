using PromptlyNote.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PromptlyNote.Core.DTOs
{
    public class ToDoTaskDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? DueDate { get; set; }
        public bool IsCompleted { get; set; } = false;
        public string CategoryId { get; set; } = string.Empty;
        public Category? Category { get; set; }

        public string UserId { get; set; } = string.Empty;
        public UserDto? User { get; set; }

        public string TaskListId { get; set; } = string.Empty;
        public TaskListDto? TaskList { get; set; }
        public List<SubTask> SubTasks { get; set; } = [];
    }
}
