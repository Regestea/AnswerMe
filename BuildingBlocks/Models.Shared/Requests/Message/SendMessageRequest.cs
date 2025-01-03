﻿using System.ComponentModel.DataAnnotations;
using Models.Shared.Attribute;
using Models.Shared.Requests.Shared;

namespace Models.Shared.Requests.Message
{
    public class SendMessageRequest
    {
        public string? Text { get; set; }

        public Guid? ReplyMessageId { get; set; }

        public string? GroupInvitationToken { get; set; }

        [MaxItems(20)]
        public List<TokenRequest>? MediaTokenList { get; set; }
    }
}
