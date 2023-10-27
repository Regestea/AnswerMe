using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnswerMe.Domain.Common;

namespace AnswerMe.Domain.Entities
{
    public class RoomLastSeen:BaseEntity
    { 
        public Guid UserId { get; set; }
        public Guid RoomId { get; set; }
        public DateTimeOffset LastSeenUtc { get; set; }
    }
}
