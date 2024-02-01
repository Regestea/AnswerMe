using Models.Shared.Responses.Media;
using Models.Shared.Responses.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Shared.Responses.Message
{
    public class MessageResponse
    {
        public Guid id { get; set; }

        public required Guid RoomChatId { get; set; }
        
        public DateTimeOffset? CreatedDate { get; set; }
        public DateTimeOffset? ModifiedDate { get; set; }

        public string? Text { get; set; }

        public string? GroupInviteToken { get; set; }

        public required PreviewUserResponse UserSender { get; set; }

        public Guid? ReplyMessageId { get; set; }

        public List<MediaResponse>? MediaList { get; set; }
    }
}
