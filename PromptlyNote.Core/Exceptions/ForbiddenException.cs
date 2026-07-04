using System;
using System.Collections.Generic;
using System.Text;

namespace PromptlyNote.Core.Exceptions
{
    public class ForbiddenException(string message = "Access denied") : Exception(message)
    {
    }
}
