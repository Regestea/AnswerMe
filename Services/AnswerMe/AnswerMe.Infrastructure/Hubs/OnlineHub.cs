using AnswerMe.Application.Common.Interfaces;
using AnswerMe.Application.DTOs.Room;
using AnswerMe.Application.DTOs.User;
using AnswerMe.Domain.Entities;
using AnswerMe.Infrastructure.Persistence;
using IdentityServer.Shared.Client.Attributes;
using IdentityServer.Shared.Client.Repositories.Interfaces;
using IdentityServer.Shared.Client.Service;
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
        private IAuthenticationService _authenticationService;

        public OnlineHub(IJwtTokenRepository jwtTokenRepository, ICacheRepository cacheRepository,
            AnswerMeDbContext context, IAuthenticationService authenticationService)
        {
            _jwtTokenRepository = jwtTokenRepository;
            _cacheRepository = cacheRepository;
            _context = context;
            _authenticationService = authenticationService;
        }


        /// <summary>
        /// Overrides the OnConnectedAsync method of the base class. It is executed when a client
        /// connects to the hub.
        /// Requires GroupId in the header for authorization.
        /// Stores the user's online status in the cache repository.
        /// Sends an "OnlineUserMessage" to the online contacts if there are any.
        /// </summary>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public override async Task OnConnectedAsync()
        {
            var jwtToken = _jwtTokenRepository.GetJwtToken();
            var isAuthenticated = await _authenticationService.IsAuthenticatedAsync(jwtToken);

            if (isAuthenticated)
            {
                var userDto = _jwtTokenRepository.ExtractUserDataFromToken(jwtToken);

                await _cacheRepository.SetAsync("Online-" + userDto.id, new UserOnlineDto()
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
                    .Where(chat =>
                        (chat.User1Id == userDto.id || chat.User2Id == userDto.id) && (chat.User1Id != chat.User2Id))
                    .Select(chat => chat.User1Id == userDto.id ? chat.User2Id : chat.User1Id)
                    .ToListAsync();

                var onlineContactConnectionIdList = new List<string>();

                foreach (var contactId in userContactIds)
                {
                    var onlineContact = await _cacheRepository.GetAsync<UserOnlineDto>("Online-"+contactId);
                    if (onlineContact != null)
                    {
                        onlineContactConnectionIdList.Add(onlineContact.ConnectionId);
                    }
                }

                if (onlineContactConnectionIdList.Any())
                {
                    await Clients.Clients(onlineContactConnectionIdList).SendAsync("UserWentOnline", userDto.id);
                }

                
                await base.OnConnectedAsync();
            }

            if (!isAuthenticated)
            {
                Context.Abort();
            }
        }

        /// <summary>
        /// Method executed when a client is disconnected from the system.
        /// </summary>
        /// <param name="exception">The exception, if any, that caused the client to disconnect.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <remarks>
        /// This method is decorated with [AuthorizeByIdentityServer] attribute, which ensures that the client is
        /// authorized using Identity Server.
        /// This method removes the user's connection ID from the cache and performs additional cleanup tasks if necessary
        /// .
        /// If a user is associated with the connection ID, this method removes the user's entry from the cache, adds
        /// a new record to the OnlineStatusUsers table with the user's ID and the current UTC date and time, and saves
        /// the changes to the database using the DbContext.
        /// Finally, the base implementation of the OnDisconnectedAsync method is invoked to complete the disconnection
        /// process.
        /// </remarks>
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userOnlineDto = await _cacheRepository.GetAsync<UserOnlineDto>(Context.ConnectionId);

            if (userOnlineDto != null)
            {
                await _cacheRepository.RemoveAsync(Context.ConnectionId);
                
                await _cacheRepository.RemoveAsync("Online-"+userOnlineDto.UserId);
                
                await _context.OnlineStatusUsers.AddAsync(new OnlineStatusUser()
                    { UserId = userOnlineDto.UserId, LastOnlineDateTime = DateTimeOffset.UtcNow });
                await _context.SaveChangesAsync();

                var userContactIds = await _context.PrivateChats
                    .Where(chat =>
                        (chat.User1Id == userOnlineDto.UserId || chat.User2Id == userOnlineDto.UserId) &&
                        (chat.User1Id != chat.User2Id))
                    .Select(chat => chat.User1Id == userOnlineDto.UserId ? chat.User2Id : chat.User1Id)
                    .ToListAsync();

                var onlineContactConnectionIdList = new List<string>();

                foreach (var contactId in userContactIds)
                {
                    var onlineContact = await _cacheRepository.GetAsync<UserOnlineDto>("Online-"+contactId);
                    if (onlineContact != null)
                    {
                        onlineContactConnectionIdList.Add(onlineContact.ConnectionId);
                    }
                }

                if (onlineContactConnectionIdList.Any())
                {
                    await Clients.Clients(onlineContactConnectionIdList)
                        .SendAsync("UserWentOffline", userOnlineDto.UserId);
                }
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}