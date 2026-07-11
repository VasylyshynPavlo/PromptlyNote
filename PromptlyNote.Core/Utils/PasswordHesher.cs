namespace PromptlyNote.Core.Utils
{
    public static class PasswordHesher
    {
        private const int WorkFactor = 13;

        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, workFactor: WorkFactor);
        }
        public static bool Verify(string password, string passwordHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }

        public static bool NeedsRehash(string passwordHash)
        {
            return BCrypt.Net.BCrypt.PasswordNeedsRehash(passwordHash, WorkFactor);
        }
    }
}
