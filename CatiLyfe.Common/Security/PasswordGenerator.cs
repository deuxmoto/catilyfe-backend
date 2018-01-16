namespace CatiLyfe.Common.Security
{
    using System;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;

    using CatiLyfe.Common.Exceptions;
    using CatiLyfe.Common.Logging;

    /// <summary>
    /// The password generator.
    /// </summary>
    public static class PasswordGenerator
    {
        /// <summary>
        /// Hashes a password.
        /// </summary>
        /// <param name="salt">The password salt.</param>
        /// <param name="password">The password.</param>
        /// <returns>The hashed password.</returns>
        public static byte[] HashPassword(byte[] salt, string password)
        {
            var passwordBytes = Encoding.Unicode.GetBytes(password);

            var binary = salt.Concat(passwordBytes).ToArray();

            using (var hash = SHA512Managed.Create())
            {
                return hash.ComputeHash(binary);
            }
        }

        /// <summary>
        /// Generate random bytes for a token.
        /// </summary>
        /// <param name="length">The length in bytes.</param>
        /// <returns>The random bytes.</returns>
        public static byte[] GenerateRandom(int length)
        {
            var bytes = new byte[length];
            using (var random = RandomNumberGenerator.Create())
            {
                random.GetNonZeroBytes(bytes);
            }

            return bytes;
        }

        /// <summary>
        /// test two passwords for equality.
        /// </summary>
        /// <param name="actualPassword">The actual password.</param>
        /// <param name="testPassword">The test password.</param>
        /// <returns>True on match.</returns>
        public static bool IsMatch(byte[] actualPassword, byte[] testPassword)
        {
            if (actualPassword.Length != testPassword.Length)
            {
                return false;
            }

            for (var i = 0; i < actualPassword.Length; i++)
            {
                if (actualPassword[i] != testPassword[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
