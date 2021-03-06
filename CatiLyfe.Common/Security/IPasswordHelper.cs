﻿namespace CatiLyfe.Common.Security
{
    using CatiLyfe.Common.Exceptions;

    /// <summary>
    /// The PasswordHelper interface.
    /// </summary>
    public interface IPasswordHelper
    {
        /// <summary>
        /// Hashes a password.
        /// </summary>
        /// <param name="password">The password.</param>
        /// <returns>The hashed password.</returns>
        byte[] HashPassword(string password);

        /// <summary>
        /// Generate random bytes for a token.
        /// </summary>
        /// <param name="length">The length in bytes.</param>
        /// <returns>The random bytes.</returns>
        byte[] GenerateTokenBytes(int length);

        /// <summary>
        /// Test two passwords for equality.
        /// </summary>
        /// <param name="actualPassword">The actual password.</param>
        /// <param name="testPassword">The test password.</param>
        /// <returns>True if the password matches.</returns>
        bool IsMatch(byte[] actualPassword, byte[] testPassword);
    }
}
