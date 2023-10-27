using AnswerMe.Application.Common.Interfaces;
using AnswerMe.Application.DTOs.Room;
using AnswerMe.Application.DTOs.User;
using AnswerMe.Domain.Entities;
using AnswerMe.Infrastructure.Persistence;
using IdentityServer.Shared.Client.Attributes;
using IdentityServer.Shared.Client.Repositories.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SignalRSwaggerGen.Attributes;

namespace AnswerMe.Infrastructure.Hubs
{
    [SignalRHub("/Online-User")]
    public class OnlineHub : Hub
    {
        private readonly IJwtTokenRepository _jwtTokenRepository;
        private ICacheRepository _cacheRepository;
        private AnswerMeDbContext _context;
        private IHubContext<PrivateRoomHub> _privateRoomHub;

        public OnlineHub(IJwtTokenRepository jwtTokenRepository, ICacheRepository cacheRepository, AnswerMeDbContext context, IHubContext<PrivateRoomHub> privateRoomHub)
        {
            _jwtTokenRepository = jwtTokenRepository;
            _cacheRepository = cacheRepository;
            _context = context;
            _privateRoomHub = privateRoomHub;
        }



        /// <summary>
        ///GroupId in header is required
        /// </summary>
        [AuthorizeByIdentityServer]
        public override async Task OnConnectedAsync()
        {
            var jwtToken = _jwtTokenRepository.GetJwtToken();
            var userDto = _jwtTokenRepository.ExtractUserDataFromToken(jwtToken);

            await _cacheRepository.SetAsync(userDto.id.ToString(), new UserOnlineDto()
            {
                UserId = userDto.id,
                ConnectionId = Context.ConnectionId
            }, TimeSpan.FromDays(7));

            await _cacheRepository.SetAsync(Context.ConnectionId, new UserOnlineDto()
            {
                UserId = userDto.id,
                ConnectionId = Context.ConnectionId
            }, TimeSpan.FromDays(7));


            var userContactIds = await _context.PrivateChats
                .Where(chat => (chat.User1Id == userDto.id || chat.User2Id == userDto.id) && (chat.User1Id != chat.User2Id))
                .Select(chat => chat.User1Id == userDto.id ? chat.User2Id : chat.User1Id)
                .Distinct()
                .ToListAsync();

            var onlineContactConnectionIdList = new List<string>();

            foreach (var contactId in userContactIds)
            {
                var inRoomOnlineUser = await _cacheRepository.GetAsync<RoomConnectionDto>(contactId.ToString());
                if (inRoomOnlineUser != null)
                {
                    onlineContactConnectionIdList.Add(inRoomOnlineUser.ConnectionId);
                }
            }
            
            if (onlineContactConnectionIdList.Any())
            {
                await _privateRoomHub.Clients.Clients(onlineContactConnectionIdList).SendAsync("OnlineUserMessage", userDto.id);
            }

            await base.OnConnectedAsync();
        }

        [AuthorizeByIdentityServer]
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userOnlineDto = await _cacheRepository.GetAsync<UserOnlineDto>(Context.ConnectionId);

            await _cacheRepository.RemoveAsync(Context.ConnectionId);

            if (userOnlineDto != null)
            {
                await _cacheRepository.RemoveAsync(userOnlineDto.UserId.ToString());
                await _context.OnlineStatusUsers.AddAsync(new OnlineStatusUser() { UserId = userOnlineDto.UserId, LastOnlineDateTime = DateTimeOffset.UtcNow });
                await _context.SaveChangesAsync();
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
