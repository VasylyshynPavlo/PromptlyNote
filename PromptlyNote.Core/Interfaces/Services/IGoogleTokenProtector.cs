namespace PromptlyNote.Core.Interfaces.Services
{
    public interface IGoogleTokenProtector
    {
        string Protect(string plainText);
        string Unprotect(string cipherText);
    }
}
