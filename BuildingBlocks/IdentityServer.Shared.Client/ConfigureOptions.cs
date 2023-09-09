using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServer.Shared.Client
{
    public class ConfigureOptions
    {
        public string RedisConnectionString { get; set; } = null!;

        public string RedisInstanceName { get; set; } = null!;

        public string IdentityServerGrpcUrl { get; set; } = null!;

        public string IssuerUrl { get; set; } = null!;

    }
}
