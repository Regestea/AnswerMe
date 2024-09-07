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
    public class GroupHubService : IGroupHubService
    {
        private IHubContext<GroupRoomHub> _groupHubContext;
        private IHubContext<OnlineHub> _onlineHubContext;
        private AnswerMeDbContext _context;
        private ICacheRepository _cacheRepository;

        public GroupHubService(IHubContext<GroupRoomHub> groupHubContext, IHubContext<OnlineHub> onlineHubContext,
            AnswerMeDbContext context, ICacheRepository cacheRepository)
        {
            _groupHubContext = groupHubContext;
            _onlineHubContext = onlineHubContext;
            _context = context;
            _cacheRepository = cacheRepository;
        }

        public async Task SendMessageAsync(Guid loggedInUserId, Guid roomId, MessageResponse messageResponse)
        {
            await _groupHubContext.Clients
                .Group(roomId.ToString())
                .SendAsync("NewGRMessage", messageResponse);

            var onlineMemberConnectionIdList = await OnlineMemberConnectionIdListAsync(roomId);
            if (onlineMemberConnectionIdList.Any())
            {
                var roomNotify = await GetRoomNotifyAsync(loggedInUserId, roomId, messageResponse.Text);


                await _onlineHubContext.Clients
                    .Clients(onlineMemberConnectionIdList)
                    .SendAsync("NotifyNewGRMessage", roomNotify);
            }
        }
    

    public async Task UpdateMessageAsync(Guid loggedInUserId, Guid roomId, MessageResponse messageResponse)
        {
            await _groupHubContext.Clients
                .Group(roomId.ToString())
                .SendAsync("EditGRMessage", messageResponse);
            
            var onlineMemberConnectionIdList = await OnlineMemberConnectionIdListAsync(roomId);
            if (onlineMemberConnectionIdList.Any())
            {
                var roomNotify = await GetRoomNotifyAsync(loggedInUserId, roomId, messageResponse.Text);
                
                await _onlineHubContext.Clients
                    .Clients(onlineMemberConnectionIdList)
                    .SendAsync("NotifyEditGRMessage", roomNotify);
            }
        }

        public async Task RemoveMessageAsync(Guid loggedInUserId, Guid roomId, Guid messageId)
        {
            await _groupHubContext.Clients
                .Group(roomId.ToString())
                .SendAsync("RemoveGRMessage", new IdResponse
                {
                    FieldName = nameof(messageId),
                    Id = messageId
                });

            var lastMessageText = await _context.Messages
                .Where(x => x.RoomChatId == roomId)
                .OrderByDescending(x => x.CreatedDate)
                .Select(x => x.Text)
                .FirstOrDefaultAsync();
            
            var onlineMemberConnectionIdList = await OnlineMemberConnectionIdListAsync(roomId);
            if (onlineMemberConnectionIdList.Any())
            {
                var roomNotify = await GetRoomNotifyAsync(loggedInUserId, roomId, lastMessageText);
                
                await _onlineHubContext.Clients
                    .Clients(onlineMemberConnectionIdList)
                    .SendAsync("NotifyRemoveGRMessage", roomNotify);
            }
        }

        private async Task<RoomNotifyResponse> GetRoomNotifyAsync(Guid loggedInUserId, Guid roomId, string? messageText)
        {
            var isInRoom = await _cacheRepository.GetAsync<RoomConnectionDto>("GR-" + loggedInUserId);
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
        
        private async Task<List<string>> OnlineMemberConnectionIdListAsync(Guid roomId)
        {
            var userContactIds = await _context.UserGroups
                .Where(x => x.GroupId == roomId)
                .Select(x => x.UserId)
                .ToListAsync();

            var onlineContactConnectionIdList = new List<string>();

            foreach (var contactId in userContactIds)
            {
                var onlineContact = await _cacheRepository.GetAsync<UserOnlineDto>("Online-"+contactId.ToString());
                if (onlineContact != null)
                {
                    onlineContactConnectionIdList.Add(onlineContact.ConnectionId);
                }
            }

            return onlineContactConnectionIdList;
        }
    }
}