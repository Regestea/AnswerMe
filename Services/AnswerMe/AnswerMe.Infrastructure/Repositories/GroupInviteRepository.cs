using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnswerMe.Application.Extensions;
using AnswerMe.Domain.Entities;
using AnswerMe.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Models.Shared.RepositoriesResponseTypes;
using Models.Shared.Requests.Group;
using Models.Shared.Responses.Group;
using Models.Shared.Responses.Shared;
using OneOf.Types;
using Security.Shared.Extensions;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using Models.Shared.OneOfTypes;
using AnswerMe.Application.Common.Interfaces;

namespace AnswerMe.Infrastructure.Repositories
{
    public class GroupInviteRepository : IGroupInviteRepository
    {
        private readonly AnswerMeDbContext _context;

        public GroupInviteRepository(AnswerMeDbContext context)
        {
            _context = context;
        }

        public async Task<ReadResponse<GroupResponse>> GetGroupPreviewAsync(string inviteToken)
        {
            var groupInvitation = await _context.GroupInvitations.SingleOrDefaultAsync(x => x.Token == inviteToken);

            if (groupInvitation == null)
            {
                return new NotFound();
            }

            var groupResponse = await _context.GroupChats
                .Where(x => x.id == groupInvitation.GroupId)
                .Select(x => new GroupResponse()
                {
                    Id = x.id,
                    CreatedDate = x.CreatedDate,
                    ModifiedDate = x.ModifiedDate,
                    Name = x.Name,
                    RoomImage = FileStorageHelper.GetUrl(x.RoomImage)
                }).SingleAsync();

            return new Success<GroupResponse>(groupResponse);
        }

        public async Task<CreateResponse<TokenResponse>> CreateAsync(Guid loggedInUserId, Guid groupId, CreateInviteTokenRequest request)
        {
            var existGroup = await _context.GroupChats.IsAnyAsync(x => x.id == groupId);

            if (!existGroup)
            {
                return new NotFound();
            }

            var isGroupAdmin = await _context.GroupAdmins.IsAnyAsync(x => x.UserId == loggedInUserId && x.RoomId == groupId);

            if (!isGroupAdmin)
            {
                return new AccessDenied();
            }

            if (request.ExpirationDate < DateTimeOffset.UtcNow || request.ExpirationDate > DateTimeOffset.UtcNow.AddDays(1))
            {
                return new ValidationFailed() { Field = nameof(request.ExpirationDate), Error = "The expiration date should have at least one day to expiry" };
            }

            var groupInvitation = new GroupInvite()
            {
                id = Guid.NewGuid(),
                CreatedDate = DateTimeOffset.UtcNow,
                GroupId = groupId,
                ExpirationDate = request.ExpirationDate,
                UserCount = request.UserCount,
                Token = TokenGenerator.GenerateNewRngCrypto()
            };

            await _context.GroupInvitations.AddAsync(groupInvitation);
            await _context.SaveChangesAsync();

            return new Success<TokenResponse>(new TokenResponse() { FieldName = "GroupInviteToken", Token = groupInvitation.Token });

        }

        public async Task<CreateResponse<IdResponse>> JoinGroupAsync(Guid loggedInUserId, string inviteToken)
        {
            var groupInvitation = _context.GroupInvitations
                .FirstOrDefault(x => x.Token == inviteToken && x.ExpirationDate >= DateTimeOffset.UtcNow);

            if (groupInvitation == null)
            {
                return new NotFound();
            }

            if (groupInvitation.UserCount < 1)
            {
                return new ValidationFailed() { Field = nameof(inviteToken), Error = "max user reached ask for new invite" };
            }

            await _context.UserGroups.AddAsync(new UserGroup() { UserId = loggedInUserId, GroupId = groupInvitation.GroupId });
            groupInvitation.UserCount -= 1;

            _context.GroupInvitations.Update(groupInvitation);

            await _context.SaveChangesAsync();

            return new Success<IdResponse>(new IdResponse() { FieldName = nameof(groupInvitation.GroupId), Id = groupInvitation.GroupId });
        }
    }
}
