using PromptlyNote.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace PromptlyNote.Core.Constants
{
    public static class NewUserConfiguration
    {
        public static ReadOnlyCollection<DefaultTaskListTemplate> DefaultList { get; } = new(
            [
            new("Personal", "Personal tasks", IconNames.Personal),
            new("Work", "Work-related tasks", IconNames.Briefcase),
            new("Ideas", "Ideas and brainstorming", IconNames.Lightbulb),
            new("Important", "Important tasks", IconNames.Star),
            new("To-Do", "Tasks to complete", IconNames.CheckCircle)
            ]
        );

        public static ReadOnlyCollection<DefaultCategoryTemplate> DefaultCategories { get; } = new(
            [
            new("Yellow", "#FFFF00"),
            new("Green", "#107C41"),
            new("Red", "#A80000"),
            new("Lilac", "#B4009E"),
            new("Orange", "#ED592B"),
            new("Blue", "#0078D4"),
            ]
        );

    }
}
