using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServer.Shared.Client.DTOs
{
    public class UserDto
    {
        public Guid id { get; set; }
        public string IdName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
    }
}
