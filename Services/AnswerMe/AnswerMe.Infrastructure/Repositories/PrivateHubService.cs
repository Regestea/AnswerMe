using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnswerMe.Application.Common.Interfaces;
using AnswerMe.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;
using Models.Shared.Responses.Message;

namespace AnswerMe.Infrastructure.Repositories
{
    public class PrivateHubService : IPrivateHubService
    {
        private IHubContext<PrivateRoomHub> _hubContext;

        public PrivateHubService(IHubContext<PrivateRoomHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendMessageAsync(Guid roomId, MessageResponse messageResponse)
        {
            await _hubContext.Clients.Group(roomId.ToString()).SendAsync("ReceivePrivateMessage", messageResponse);
        }

        public async Task UpdateMessageAsync(Guid roomId, MessageResponse messageResponse)
        {
            await _hubContext.Clients.Group(roomId.ToString()).SendAsync("UpdatePrivateMessage", messageResponse);
        }

        public async Task RemoveMessageAsync(Guid roomId, Guid messageId)
        {
            await _hubContext.Clients.Group(roomId.ToString()).SendAsync("RemovePrivateMessage", messageId);
        }
    }
}
