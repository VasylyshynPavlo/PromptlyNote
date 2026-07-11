using System;
using System.Collections.Generic;
using System.Text;

namespace PromptlyNote.Core.Exceptions
{
    public abstract class ApiException : Exception
    {
        public abstract int StatusCode { get; }

        protected ApiException(string message)
            : base(message)
        {
        }

        protected ApiException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
