using System.Security.Cryptography;
using System.Text;

namespace IdentityServer.Api.Extensions
{
    /// <summary>
    /// Provides methods for hashing passwords.
    /// </summary>
    public static class PasswordHash
    {
        public static string Hash(string password)
        {
                using var sha256 = SHA512.Create();
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                var hashedPassword = Convert.ToBase64String(hashedBytes);
            return hashedPassword;
        }
    }
}
