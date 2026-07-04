using System.Security.Cryptography;

namespace PromptlyNote.Core.Utils
{
    public static class PasswordGenerator
    {
        private const string Lowercase = "abcdefghijklmnopqrstuvwxyz";
        private const string Uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string Digits = "0123456789";
        private const string Symbols = "!@#$%^&*()-_=+[]{}<>?/";

        private static readonly string AllCharacters =
            Lowercase + Uppercase + Digits + Symbols;

        public static string Generate(int length = 16)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(length);
            char[] password = new char[length];

            for (int i = 0; i < length; i++)
            {
                password[i] = AllCharacters[
                    RandomNumberGenerator.GetInt32(AllCharacters.Length)
                ];
            }

            return new string(password);
        }
    }
}
