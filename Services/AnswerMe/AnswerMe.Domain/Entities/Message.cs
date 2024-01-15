using AnswerMe.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnswerMe.Domain.Entities
{
    public class Message : BaseEntity
    {
        public Guid RoomChatId { get; set; }

        public string? Text { get; set; }

        public Guid UserSenderId { get; set; }

        public string? GroupInvitationToken { get; set; }

        public Guid? ReplyMessageId { get; set; }

        public List<Media>? MediaList { get; set; }
    }
}
