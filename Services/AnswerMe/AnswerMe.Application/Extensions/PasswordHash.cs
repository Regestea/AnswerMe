using System.Security.Cryptography;
using System.Text;

namespace AnswerMe.Application.Extensions
{
    /// <summary>
    /// Provides methods for hashing passwords.
    /// </summary>
    public static class PasswordHash
    {
        /// <summary>
        /// Computes a SHA-512 hash for the given password using UTF-8 encoding and returns the hashed password as a Base64 encoded string.
        /// </summary>
        /// <param name="password">The password to be hashed.</param>
        /// <returns>The hashed password as a Base64 encoded string.</returns>
        public static string Hash(string password)
        {
            /// <summary>
                /// Represents a SHA-512 object for computing the SHA-512 hash value.
                /// </summary>
                /// <returns>A new instance of the SHA-512 object.</returns>
                using var sha256 = SHA512.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            /// <summary>
                /// Convert the given hashed bytes to a base64 string representation.
                /// </summary>
                /// <param name="hashedBytes">The hashed bytes to convert.</param>
                /// <returns>The base64 string representation of the hashed bytes.</returns>
                var hashedPassword = Convert.ToBase64String(hashedBytes);
            return hashedPassword;
        }
    }
}
