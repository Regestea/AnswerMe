using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnswerMe.Domain.Common;

namespace AnswerMe.Domain.Entities
{
    public class RoomAdmin : BaseEntity
    {
        public Guid RoomId { get; set; }
        public Guid UserId { get; set; }

        [ForeignKey("RoomId")]
        public Room Room { get; set; } = null!;

        [ForeignKey("UserId")]
        public User User { get; set; } = null!;
    }
}
