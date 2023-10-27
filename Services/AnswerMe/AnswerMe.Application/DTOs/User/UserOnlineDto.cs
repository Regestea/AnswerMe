using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnswerMe.Application.DTOs.User
{
    public class UserOnlineDto
    {
        public Guid UserId { get; set; }
        public string ConnectionId { get; set; } = null!;
    }
}
