using System;
using System.Collections.Generic;
using System.Text;

namespace PromptlyNote.Core.Interfaces.Services
{
    public interface IGoogleTokenProtector
    {
        string Protect(string plainText);
        string Unprotect(string cipherText);
    }
}
