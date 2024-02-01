using AnswerMe.Application.Common.Interfaces;
using AnswerMe.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;
using Models.Shared.Responses.Message;

namespace AnswerMe.Infrastructure.Services
{
    public class GroupHubService : IGroupHubService
    {
        private IHubContext<GroupRoomHub> _hubContext;

        public GroupHubService(IHubContext<GroupRoomHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendMessageAsync(Guid roomId, MessageResponse messageResponse)
        {
            await _hubContext.Clients.Group(roomId.ToString()).SendAsync("ReceiveGroupMessage", messageResponse);
        }

        public async Task UpdateMessageAsync(Guid roomId, MessageResponse messageResponse)
        {
            await _hubContext.Clients.Group(roomId.ToString()).SendAsync("UpdateGroupMessage", messageResponse);
        }

        public async Task RemoveMessageAsync(Guid roomId, Guid messageId)
        {
            await _hubContext.Clients.Group(roomId.ToString()).SendAsync("RemoveGroupMessage", messageId);
        }
    }
}
