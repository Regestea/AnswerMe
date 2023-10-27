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
        ///RoomId in header is required
        /// </summary>
        [AuthorizeByIdentityServer]
        public override async Task OnConnectedAsync()
        {
            try
            {
                if (Context.GetHttpContext().Request.Headers.TryGetValue("RoomId", out var roomIdValues))
                {
                    var roomId = Guid.Parse(roomIdValues.ToString());

                    var jwtToken = _jwtTokenRepository.GetJwtToken();
                    var userDto = _jwtTokenRepository.ExtractUserDataFromToken(jwtToken);


                    var existRoom = await _context.PrivateChats.IsAnyAsync(x => x.id == roomId && (x.User1Id == userDto.id || x.User2Id == userDto.id));
                    if (!existRoom)
                    {
                        Context.Abort();
                    }

                    await Groups.AddToGroupAsync(Context.ConnectionId, roomId.ToString());

                    await _cacheRepository.SetAsync(Context.ConnectionId,
                         new RoomConnectionDto() { ConnectionId = Context.ConnectionId, RoomId = roomId, UserId = userDto.id },
                         TimeSpan.FromDays(7));

                    await _cacheRepository.SetAsync(userDto.id.ToString(),
                        new RoomConnectionDto() { ConnectionId = Context.ConnectionId, RoomId = roomId, UserId = userDto.id },
                        TimeSpan.FromDays(7));

                    await base.OnConnectedAsync();
                }
                else
                {
                    // If the "RoomId" header is not present, disallow the connection.
                    Context.Abort();
                }
            }
            catch (Exception exception)
            {
                Context.Abort();
            }
        }

        [AuthorizeByIdentityServer]
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var roomConnectionDto = await _cacheRepository.GetAsync<RoomConnectionDto>(Context.ConnectionId);
            if (roomConnectionDto != null)
            {
                await Groups.RemoveFromGroupAsync(roomConnectionDto.ConnectionId, roomConnectionDto.RoomId.ToString());

                await _cacheRepository.RemoveAsync(Context.ConnectionId);
                await _cacheRepository.RemoveAsync(roomConnectionDto.UserId.ToString());

                var roomLastSeen = await _context.RoomLastSeen
                     .SingleOrDefaultAsync(x =>
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
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
