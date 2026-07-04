using System;
using System.Collections.Generic;
using System.Text;

namespace PromptlyNote.Core.Models
{
    public class DefaultTaskListTemplate
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string IconName { get; set; } = string.Empty;

        public DefaultTaskListTemplate(string name, string description, string iconName)
        {
            Name = name;
            Description = description;
            IconName = iconName;
        }
    }
}
