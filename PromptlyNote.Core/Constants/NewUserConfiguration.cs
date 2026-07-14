using PromptlyNote.Core.Models;
using System.Collections.ObjectModel;

namespace PromptlyNote.Core.Constants
{
    public static class NewUserConfiguration
    {
        public static ReadOnlyCollection<DefaultTaskListTemplate> DefaultList { get; } = new(
            [
            new("Personal", "Personal tasks", "person"),
            new("Work", "Work-related tasks", "work"),
            new("Ideas", "Ideas and brainstorming", "lightbulb"),
            new("Important", "Important tasks", "star"),
            new("Someday", "Tasks for the future", "watch_later")
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
