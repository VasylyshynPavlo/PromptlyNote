using System;
using System.Collections.Generic;
using System.Text;

public class InternalException : Exception
{
    public InternalException(string message = "An internal error occurred.") : base(message)
    {
    }

    public InternalException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
