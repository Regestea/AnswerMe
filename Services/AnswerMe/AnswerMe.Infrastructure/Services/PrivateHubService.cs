using AnswerMe.Application.Common.Interfaces;
using AnswerMe.Application.DTOs.Room;
using AnswerMe.Application.DTOs.User;
using AnswerMe.Infrastructure.Hubs;
using AnswerMe.Infrastructure.Persistence;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Models.Shared.Responses.Message;

namespace AnswerMe.Infrastructure.Services
{
    public class PrivateHubService : IPrivateHubService
    {
        private IHubContext<PrivateRoomHub> _privateHubContext;
        private IHubContext<OnlineHub> _onlineHubContext;
        private ICacheRepository _cacheRepository;
        private AnswerMeDbContext _context;

        public PrivateHubService(IHubContext<PrivateRoomHub> privateHubContext, IHubContext<OnlineHub> onlineHubContext,
            ICacheRepository cacheRepository, AnswerMeDbContext context)
        {
            _privateHubContext = privateHubContext;
            _onlineHubContext = onlineHubContext;
            _cacheRepository = cacheRepository;
            _context = context;
        }

        public async Task SendMessageAsync(Guid loggedInUserId, Guid roomId, MessageResponse messageResponse)
        {
            await _privateHubContext.Clients.Group(roomId.ToString())
                .SendAsync("ReceivePrivateMessage", messageResponse);

            var userContactIds = await _context.PrivateChats
                .Where(x => x.id == roomId)
                .Select(chat => chat.User1Id == messageResponse.UserSender.Id ? chat.User2Id : chat.User1Id)
                .ToListAsync();

            var onlineContactConnectionIdList = new List<string>();

            foreach (var contactId in userContactIds)
            {
                var onlineContact = await _cacheRepository.GetAsync<UserOnlineDto>(contactId.ToString());
                if (onlineContact != null)
                {
                    onlineContactConnectionIdList.Add(onlineContact.ConnectionId);
                }
            }

            if (onlineContactConnectionIdList.Any())
            {
                var isInRoom = await _cacheRepository.GetAsync<RoomConnectionDto>("PV-"+loggedInUserId);
                var roomNotify = new RoomNotifyResponse() { RoomId = roomId, MessageGlance = "", TotalUnRead = 0 };

                if (!string.IsNullOrWhiteSpace(messageResponse.Text))
                {
                    roomNotify.MessageGlance = messageResponse.Text[..Math.Min(10, messageResponse.Text.Length)];
                }

                if (isInRoom == null)
                {
                    var lastSeen =await _context.RoomLastSeen
                        .Where(x => x.RoomId == roomId && x.UserId == loggedInUserId)
                        .FirstOrDefaultAsync();

                    if (lastSeen != null)
                    {
                        var unReadCount =await _context.Messages
                            .Where(x => x.RoomChatId == roomId && x.CreatedDate > lastSeen.LastSeenUtc)
                            .CountAsync();

                        roomNotify.TotalUnRead = unReadCount;
                    }
                }


                await _onlineHubContext.Clients.Clients(onlineContactConnectionIdList)
                    .SendAsync("RoomNotify", roomNotify);
            }
        }

        public async Task UpdateMessageAsync(Guid roomId, MessageResponse messageResponse)
        {
            await _privateHubContext.Clients.Group(roomId.ToString()).SendAsync("UpdatePrivateMessage", messageResponse);
        }

        public async Task RemoveMessageAsync(Guid roomId, Guid messageId)
        {
            await _privateHubContext.Clients.Group(roomId.ToString()).SendAsync("RemovePrivateMessage", messageId);
        }
    }
}