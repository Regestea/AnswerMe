using AnswerMe.Application.Common.Interfaces;
using AnswerMe.Application.DTOs.Room;
using AnswerMe.Application.DTOs.User;
using AnswerMe.Infrastructure.Hubs;
using AnswerMe.Infrastructure.Persistence;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Models.Shared.Responses.Message;
using Models.Shared.Responses.Shared;

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
            await _privateHubContext.Clients
                .Group(roomId.ToString())
                .SendAsync("NewPVMessage", messageResponse);

            var onlineUserConnectionIdList = await OnlineUserConnectionIdListAsync(roomId);

            if (onlineUserConnectionIdList.Any())
            {
                var roomNotify = await GetRoomNotifyAsync(loggedInUserId, roomId, messageResponse.Text);
                
                await _onlineHubContext.Clients
                    .Clients(onlineUserConnectionIdList)
                    .SendAsync("NotifyNewPVMessage", roomNotify);
            }
        }

        public async Task UpdateMessageAsync(Guid loggedInUserId, Guid roomId, MessageResponse messageResponse)
        {
            await _privateHubContext.Clients
                .Group(roomId.ToString())
                .SendAsync("EditPVMessage", messageResponse);
            
            var onlineUserConnectionIdList = await OnlineUserConnectionIdListAsync(roomId);

            if (onlineUserConnectionIdList.Any())
            {
                var roomNotify = await GetRoomNotifyAsync(loggedInUserId, roomId, messageResponse.Text);
                
                await _onlineHubContext.Clients
                    .Clients(onlineUserConnectionIdList)
                    .SendAsync("NotifyEditPVMessage", roomNotify);
            }
        }

        public async Task RemoveMessageAsync(Guid loggedInUserId, Guid roomId, Guid messageId)
        {
            await _privateHubContext.Clients
                .Group(roomId.ToString())
                .SendAsync("RemovePVMessage", new IdResponse()
                {
                    FieldName = nameof(messageId),
                    Id = messageId
                });
            
            var lastMessageText = await _context.Messages
                .Where(x => x.RoomChatId == roomId)
                .OrderByDescending(x => x.CreatedDate)
                .Select(x => x.Text)
                .FirstOrDefaultAsync();
            
            var onlineMemberConnectionIdList = await OnlineUserConnectionIdListAsync(roomId);
            if (onlineMemberConnectionIdList.Any())
            {
                var roomNotify = await GetRoomNotifyAsync(loggedInUserId, roomId, lastMessageText);
                
                await _onlineHubContext.Clients
                    .Clients(onlineMemberConnectionIdList)
                    .SendAsync("NotifyRemovePVMessage", roomNotify);
            }
        }

        private async Task<RoomNotifyResponse> GetRoomNotifyAsync(Guid loggedInUserId, Guid roomId, string? messageText)
        {
            var isInRoom = await _cacheRepository.GetAsync<RoomConnectionDto>("PV-" + loggedInUserId);
            var roomNotify = new RoomNotifyResponse() { RoomId = roomId, MessageGlance = "", TotalUnRead = 0 };

            if (!string.IsNullOrWhiteSpace(messageText))
            {
                roomNotify.MessageGlance = messageText[..Math.Min(10, messageText.Length)];
            }

            if (isInRoom == null)
            {
                var lastSeen = await _context.RoomLastSeen
                    .Where(x => x.RoomId == roomId && x.UserId == loggedInUserId)
                    .FirstOrDefaultAsync();

                if (lastSeen != null)
                {
                    var unReadCount = await _context.Messages
                        .Where(x => x.RoomChatId == roomId && x.CreatedDate > lastSeen.LastSeenUtc)
                        .CountAsync();

                    roomNotify.TotalUnRead = unReadCount;
                }
            }

            return roomNotify;
        }

        private async Task<List<string>> OnlineUserConnectionIdListAsync(Guid roomId)
        {
            var userIds = await _context.PrivateChats
                .Where(x => x.id == roomId)
                .Select(chat => new List<Guid> { chat.User1Id, chat.User2Id })
                .FirstOrDefaultAsync();

            var onlineContactConnectionIdList = new List<string>();

            if (userIds != null)
            {
                foreach (var userId in userIds)
                {
                    var onlineContact = await _cacheRepository.GetAsync<UserOnlineDto>("Online-"+userId.ToString());
                    if (onlineContact != null)
                    {
                        onlineContactConnectionIdList.Add(onlineContact.ConnectionId);
                    }
                }
            }
            
            return onlineContactConnectionIdList;
        }
    }
}