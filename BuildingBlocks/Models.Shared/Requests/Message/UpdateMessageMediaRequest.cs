using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Shared.Requests.Message
{
    public class UpdateMessageMediaRequest
    {
        public Guid MediaId { get; set; }

        public string MediaToken { get; set; } = null!;
    }
}
