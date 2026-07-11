using PromptlyNote.Core.Constants;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

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
        string entityName,
        [CallerArgumentExpression(nameof(value))] string paramName = "")
        {
            if (!Guid.TryParse(value, out var parsedGuid))
            {
                throw new ArgumentException(
                    ExceptionMessages.InvalidIdFormat(entityName),
                    paramName);
            }

            return parsedGuid;
        }
    }
}
