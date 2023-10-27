using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable SYSLIB0023
namespace Security.Shared.Extensions
{
    public static class TokenGenerator
    {

        public static string GenerateNewRngCrypto()
        {
            
            using (var rng = new RNGCryptoServiceProvider())
            
            {
                var bytes = new byte[50];
                rng.GetBytes(bytes);
                return Convert.ToBase64String(bytes);
            }
        }
    }
}
#pragma warning restore SYSLIB0023