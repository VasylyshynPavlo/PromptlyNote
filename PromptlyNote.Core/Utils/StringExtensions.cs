using PromptlyNote.Core.Constants;
using PromptlyNote.Core.Exceptions;
using System.Runtime.CompilerServices;

namespace PromptlyNote.Core.Utils
{
    public static class StringExtensions
    {
        public static string CapitalizeFirst(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return value;

            return char.ToUpperInvariant(value[0]) + value[1..];
        }

        public static Guid ParseToGuidWithThrow(
        this string value,
        string entityName)
        {
            if (!Guid.TryParse(value, out var parsedGuid))
            {
                throw new BadRequestException(
                    ExceptionMessages.InvalidIdFormat(entityName));
            }

            return parsedGuid;
        }
    }
}
