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

        public GroupRoomHub(AnswerMeDbContext context, IJwtTokenRepository jwtTokenRepository, ICacheRepository cacheRepository)
        {
            _context = context;
            _jwtTokenRepository = jwtTokenRepository;
            _cacheRepository = cacheRepository;
        }

        /// <summary>
        /// Overrides the base OnConnectedAsync method and performs additional logic for handling connections to the server.
        /// </summary>
        /// <remarks>
        /// This method is executed when a client connection is established with the server. It performs the following tasks:
        /// 1. Retrieves the value of the "GroupId" header from the request.
        /// 2. Parses the GroupId value into a Guid.
        /// 3. Retrieves the JWT token from the JwtTokenRepository.
        /// 4. Extracts the user data from the JWT token using the JwtTokenRepository.
        /// 5. Checks if the user is in the specified group based on the GroupId and user ID.
        /// 6. If the user is not in the group, the connection is aborted.
        /// 7. Adds the connection to the specified group using the GroupId.
        /// 8. Sets the connection data in the cache repository using the connection ID, GroupId, and user ID.
        /// 9. Calls the base OnConnectedAsync method to continue the connection process.
        /// 10. If the "GroupId" header is not present, the connection is aborted.
        /// </remarks>
        [AuthorizeByIdentityServer]
        public override async Task OnConnectedAsync()
        {
            try
            {
                if (Context.GetHttpContext()!.Request.Headers.TryGetValue("GroupId", out var groupIdValues))
                {

                        var groupId = Guid.Parse(groupIdValues.ToString());

                    var jwtToken = _jwtTokenRepository.GetJwtToken();
                    var userDto = _jwtTokenRepository.ExtractUserDataFromToken(jwtToken);


                        var isUserInGroup = await _context.UserGroups.AnyAsync(x => x.GroupId == groupId && x.UserId == userDto.id);
                    if (!isUserInGroup)
                    {
                        Context.Abort();
                    }


                    await Groups.AddToGroupAsync(Context.ConnectionId, groupId.ToString());

                    await _cacheRepository.SetAsync(Context.ConnectionId,
                         new GroupConnectionDto() { ConnectionId = Context.ConnectionId, GroupId = groupId, UserId = userDto.id },
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
        /// Overrides the OnDisconnectedAsync method of the base class, called when a client disconnects from the
        /// hub.
        /// </summary>
        /// <param name="exception">The exception that occurred, or null if there was no exception.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        [AuthorizeByIdentityServer]
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var groupConnectionDto = await _cacheRepository.GetAsync<GroupConnectionDto>(Context.ConnectionId);
            if (groupConnectionDto != null)
            {
                await Groups.RemoveFromGroupAsync(groupConnectionDto.ConnectionId, groupConnectionDto.GroupId.ToString());


                await _context.SaveChangesAsync();
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
