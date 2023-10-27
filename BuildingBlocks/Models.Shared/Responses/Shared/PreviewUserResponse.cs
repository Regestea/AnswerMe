using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Shared.Responses.Shared
{
    public class PreviewUserResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? ProfileImage { get; set; }
    }
}
