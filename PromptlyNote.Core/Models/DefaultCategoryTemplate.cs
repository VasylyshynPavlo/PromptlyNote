namespace PromptlyNote.Core.Models
{
    public class DefaultCategoryTemplate
    {
        public string Name { get; set; } = string.Empty;
        public string ColorHex { get; set; } = string.Empty;

        public DefaultCategoryTemplate(string name, string colorHex)
        {
            Name = name;
            ColorHex = colorHex;
        }
    }
}
