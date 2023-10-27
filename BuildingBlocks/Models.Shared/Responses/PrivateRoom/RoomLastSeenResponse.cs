using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Shared.Responses.PrivateRoom
{
    public class RoomLastSeenResponse
    {
        public Guid UserId { get; set; }
        public Guid RoomId { get; set; }
        public DateTimeOffset LastSeenUtc { get; set; }
    }
}
