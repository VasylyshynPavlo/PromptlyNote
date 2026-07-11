using PromptlyNote.Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PromptlyNote.Core.DTOs.Forms.Update
{
    public class UpdateToDoTaskForm
    {
        [Required(ErrorMessage = "Name is required.")]
        [MaxLength(255, ErrorMessage = "Name cannot exceed 255 characters.")]
        [MinLength(1, ErrorMessage = "Name must be at least 1 character long.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Note is required.")]
        [MaxLength(1000, ErrorMessage = "Note cannot exceed 1000 characters.")]
        public string Note { get; set; } = string.Empty;

        public DateTime? DueDate { get; set; }

        [Required(ErrorMessage = "Task status is required.")]
        public bool IsCompleted { get; set; } = false;

        public string? CategoryId { get; set; }

        [Required(ErrorMessage = "Task list ID is required.")]
        public string TaskListId { get; set; } = string.Empty;

        public List<UpdateSubTaskForm> SubTasks { get; set; } = [];
    }
}
