using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServer.Shared.Client
{
    public class ConfigureOptions
    { 
        public string RedisConnectionName { get; set; } = null!;
        public string IdentityServerUrl { get; set; } = null!;
    }
}
