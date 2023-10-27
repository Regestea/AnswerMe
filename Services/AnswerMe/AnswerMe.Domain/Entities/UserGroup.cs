using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnswerMe.Domain.Common;

namespace AnswerMe.Domain.Entities
{
    public class UserGroup:BaseEntity
    {
        public Guid GroupId { get; set; }

        public Guid UserId { get; set; }
    }
}
