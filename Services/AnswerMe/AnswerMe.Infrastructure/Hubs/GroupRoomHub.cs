using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AnswerMe.Application.Common.Interfaces;
using AnswerMe.Application.DTOs.Group;
using AnswerMe.Application.DTOs.Room;
using AnswerMe.Application.Extensions;
using AnswerMe.Domain.Entities;
using AnswerMe.Infrastructure.Persistence;
using IdentityServer.Shared.Client.Attributes;
using IdentityServer.Shared.Client.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Models.Shared.Responses.Message;
using SignalRSwaggerGen.Attributes;
using SignalRSwaggerGen.Enums;

namespace AnswerMe.Infrastructure.Hubs
{
    [SignalRHub("/Group-Chat")]
    public class GroupRoomHub : Hub
    {
        private AnswerMeDbContext _context;
        private IJwtTokenRepository _jwtTokenRepository;
        private ICacheRepository _cacheRepository;

        public GroupRoomHub(AnswerMeDbContext context, IJwtTokenRepository jwtTokenRepository,
            ICacheRepository cacheRepository)
        {
            _context = context;
            _jwtTokenRepository = jwtTokenRepository;
            _cacheRepository = cacheRepository;
        }

        /// <summary>
        /// Overrides the base OnConnectedAsync method and performs additional logic for handling connections to the server.
        /// </summary>
        [AuthorizeByIdentityServer]
        public override async Task OnConnectedAsync()
        {
            try
            {
                if (Context.GetHttpContext()!.Request.Query.TryGetValue("RoomId", out var roomIdValues))
                {
                    var groupId = Guid.Parse(roomIdValues.ToString());

                    var jwtToken = _jwtTokenRepository.GetJwtToken();
                    var userDto = _jwtTokenRepository.ExtractUserDataFromToken(jwtToken);


                    var isUserInGroup =
                        await _context.UserGroups.AnyAsync(x => x.GroupId == groupId && x.UserId == userDto.id);
                    if (!isUserInGroup)
                    {
                        Context.Abort();
                    }

                    await Groups.AddToGroupAsync(Context.ConnectionId, groupId.ToString());

                    await _cacheRepository.SetAsync(Context.ConnectionId,
                        new GroupConnectionDto()
                        {
                            ConnectionId = Context.ConnectionId,
                            GroupId = groupId,
                            UserId = userDto.id
                        },
                        TimeSpan.FromDays(7));

                    await _cacheRepository.SetAsync("GR-" + userDto.id,
                        new GroupConnectionDto()
                        {
                            ConnectionId = Context.ConnectionId,
                            GroupId = groupId,
                            UserId = userDto.id
                        },
                        TimeSpan.FromDays(7));

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

        /// <summary>
        /// Overrides the base OnDisconnectedAsync method and performs additional logic for handling disconnections from the server.
        /// </summary>
        [AuthorizeByIdentityServer]
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var groupConnectionDto = await _cacheRepository.GetAsync<GroupConnectionDto>(Context.ConnectionId);

            if (groupConnectionDto != null)
            {
                await Groups.RemoveFromGroupAsync(groupConnectionDto.ConnectionId,
                    groupConnectionDto.GroupId.ToString());

                await _cacheRepository.RemoveAsync(Context.ConnectionId);
                await _cacheRepository.RemoveAsync("GR-" + groupConnectionDto.UserId);


                var roomLastSeen = await _context.RoomLastSeen
                    .FirstOrDefaultAsync(x =>
                        x.UserId == groupConnectionDto.UserId && x.RoomId == groupConnectionDto.GroupId);

                if (roomLastSeen == null)
                {
                    await _context.RoomLastSeen.AddAsync(new RoomLastSeen()
                    {
                        UserId = groupConnectionDto.UserId,
                        RoomId = groupConnectionDto.GroupId,
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