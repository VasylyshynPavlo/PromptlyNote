using PromptlyNote.Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PromptlyNote.Core.DTOs.Forms.Create
{
    public class CreateToDoTaskForm
    {
        [Required(ErrorMessage = "Name is required.")]
        [MaxLength(255, ErrorMessage = "Name cannot exceed 255 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Note is required.")]
        [MaxLength(1000, ErrorMessage = "Note cannot exceed 1000 characters.")]
        public string Note { get; set; } = string.Empty;

        [DataType(DataType.Date, ErrorMessage = "DueDate must be a valid date.")]
        public DateTime? DueDate { get; set; }

        public string? CategoryId { get; set; }

        [Required(ErrorMessage = "Task list ID is required.")]
        public string TaskListId { get; set; } = string.Empty;
        public List<CreateSubTaskForm> SubTasks { get; set; } = [];
    }
}
