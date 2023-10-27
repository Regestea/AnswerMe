using Models.Shared.Responses.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnswerMe.Application.DTOs.Room
{
    public class PrivateRoomDto
    {
        public Guid Id { get; set; }
        public Guid ContactId { get; set; }
    }
}
