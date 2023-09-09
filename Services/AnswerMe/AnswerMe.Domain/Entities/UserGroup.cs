using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnswerMe.Domain.Entities
{
    public class UserGroup
    {
        public Guid GroupId { get; set; }

        public Guid UserId { get; set; }
    }
}
