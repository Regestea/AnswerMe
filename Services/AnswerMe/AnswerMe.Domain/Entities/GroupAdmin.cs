using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnswerMe.Domain.Entities
{
    public class GroupAdmin
    {
        public Guid RoomId { get; set; }

        public Guid UserId { get; set; }
    }
}
