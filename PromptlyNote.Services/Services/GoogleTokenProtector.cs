using Microsoft.AspNetCore.DataProtection;
using PromptlyNote.Core.Interfaces.Services;

namespace PromptlyNote.Services.Services
{
    public class GoogleTokenProtector(IDataProtectionProvider provider) : IGoogleTokenProtector
    {
        private readonly IDataProtector _protector = provider.CreateProtector("PromptlyNote.GoogleCalendar.RefreshToken");

        public string Protect(string plainText)
        {
            return _protector.Protect(plainText);
        }

        public string Unprotect(string cipherText)
        {
            return _protector.Unprotect(cipherText);
        }
    }
}
