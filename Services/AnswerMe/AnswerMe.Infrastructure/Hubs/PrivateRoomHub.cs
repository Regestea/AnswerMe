using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnswerMe.Application.Common.Interfaces;
using AnswerMe.Application.DTOs.Room;
using AnswerMe.Application.Extensions;
using AnswerMe.Domain.Entities;
using AnswerMe.Infrastructure.Persistence;
using IdentityServer.Shared.Client.Attributes;
using IdentityServer.Shared.Client.Repositories.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Models.Shared.Responses.Message;
using SignalRSwaggerGen.Attributes;

namespace AnswerMe.Infrastructure.Hubs
{
    [SignalRHub("/Private-Chat")]
    public class PrivateRoomHub : Hub
    {
        private AnswerMeDbContext _context;
        private IJwtTokenRepository _jwtTokenRepository;
        private ICacheRepository _cacheRepository;

        public PrivateRoomHub(AnswerMeDbContext context, IJwtTokenRepository jwtTokenRepository, ICacheRepository cacheRepository)
        {
            _context = context;
            _jwtTokenRepository = jwtTokenRepository;
            _cacheRepository = cacheRepository;
        }

        /// <summary>
        /// Overrides the base OnConnectedAsync method and performs additional logic for establishing a connection.
        /// </summary>
        [AuthorizeByIdentityServer]
        public override async Task OnConnectedAsync()
        {
            try
            {
                var rr = Context.GetHttpContext();
                if (Context.GetHttpContext()!.Request.Query.TryGetValue("RoomId", out var roomIdValues))
                {
                    var roomId = Guid.Parse(roomIdValues.ToString());

                        var jwtToken = _jwtTokenRepository.GetJwtToken();
                        var userDto = _jwtTokenRepository.ExtractUserDataFromToken(jwtToken);


                        var existRoom = await _context.PrivateChats.AnyAsync(x => x.id == roomId && (x.User1Id == userDto.id || x.User2Id == userDto.id));
                    if (!existRoom)
                    {
                        Context.Abort();
                    }

                    await Groups.AddToGroupAsync(Context.ConnectionId, roomId.ToString());

                    await _cacheRepository.SetAsync(Context.ConnectionId,
                         new RoomConnectionDto() { ConnectionId = Context.ConnectionId, RoomId = roomId, UserId = userDto.id },
                         TimeSpan.FromDays(7));

                    await _cacheRepository.SetAsync("PV-"+userDto.id.ToString(),
                        new RoomConnectionDto() { ConnectionId = Context.ConnectionId, RoomId = roomId, UserId = userDto.id },
                        TimeSpan.FromDays(7));
                    
                    await Clients.Group(roomId.ToString()).SendAsync("JoinedRoom", userDto.id);

                    await base.OnConnectedAsync();
                }
                else
                {
                    // If the "RoomId" header is not present, disallow the connection.
                    Context.Abort();
                }
            }
            catch (Exception)
            {
                Context.Abort();
            }
        }

        /// Method: OnDisconnectedAsync
        /// Description: Overrides the OnDisconnectedAsync method from the Hub class.
        /// Handles the disconnection of a client from the hub.
        /// Removes the client from the associated group, updates the last seen timestamp,
        /// and performs necessary cleanup.
        /// Access Modifier: public
        /// Return Type: Task
        /// Parameters:
        /// - exception: Exception? (optional parameter) - The exception that occurred during the disconnection.
        /// /
        [AuthorizeByIdentityServer]
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
                var roomConnectionDto = await _cacheRepository.GetAsync<RoomConnectionDto>(Context.ConnectionId);
            if (roomConnectionDto != null)
            {
                await Groups.RemoveFromGroupAsync(roomConnectionDto.ConnectionId, roomConnectionDto.RoomId.ToString());

                await _cacheRepository.RemoveAsync(Context.ConnectionId);
                await _cacheRepository.RemoveAsync("PV-"+roomConnectionDto.UserId);

                    var roomLastSeen = await _context.RoomLastSeen
                     .FirstOrDefaultAsync(x =>
                     x.UserId == roomConnectionDto.UserId && x.RoomId == roomConnectionDto.RoomId);
                if (roomLastSeen == null)
                {
                    await _context.RoomLastSeen.AddAsync(new RoomLastSeen()
                    {
                        UserId = roomConnectionDto.UserId,
                        RoomId = roomConnectionDto.RoomId,
                        LastSeenUtc = DateTimeOffset.UtcNow
                    });
                }
                else
                {
                    roomLastSeen.LastSeenUtc = DateTimeOffset.UtcNow;

                    _context.RoomLastSeen.Update(roomLastSeen);
                }

                await _context.SaveChangesAsync();
                
                await Clients.Group(roomConnectionDto.RoomId.ToString()).SendAsync("LeftRoom", roomConnectionDto.UserId);
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
