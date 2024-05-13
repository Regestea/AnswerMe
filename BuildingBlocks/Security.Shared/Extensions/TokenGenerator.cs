using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;


namespace Security.Shared.Extensions
{
    public static class TokenGenerator
    {

        public static string GenerateNewToken()
        {
            return (Guid.NewGuid() +""+ Guid.NewGuid()).Replace("-","");
        }
    }
}
