using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnswerMe.Domain.Common;

namespace AnswerMe.Domain.Entities
{
    public class PrivateChat:BaseEntity
    {
        public Guid User1Id { get; set; }
        public Guid User2Id { get; set; }
    }
}
