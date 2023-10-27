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
using Microsoft.EntityFrameworkCore;
using Models.Shared.OneOfTypes;
using Models.Shared.RepositoriesResponseTypes;
using Models.Shared.Requests.Shared;
using Models.Shared.Responses.Group;
using Models.Shared.Responses.PrivateRoom;
using Models.Shared.Responses.Shared;
using OneOf.Types;

namespace AnswerMe.Infrastructure.Repositories
{
    public class PrivateRoomRepository : IPrivateRoomRepository
    {
        private readonly AnswerMeDbContext _context;

        public PrivateRoomRepository(AnswerMeDbContext context)
        {
            _context = context;
        }


        public async Task<ReadResponse<PrivateRoomResponse>> GetAsync(Guid loggedInUserId, Guid roomId)
        {
            var existRoom = await _context.PrivateChats.IsAnyAsync(x => x.id == roomId);

            if (!existRoom)
            {
                return new NotFound();
            }

            var privateRoom = await _context.PrivateChats
                .Where(x => x.id == roomId)
                .SingleOrDefaultAsync();

            if (privateRoom == null)
            {
                return new AccessDenied();
            }

            var contactId = privateRoom.User1Id == loggedInUserId ? privateRoom.User1Id : privateRoom.User2Id;

            var previewContact = await _context.Users
                .Where(x => x.id == contactId)
                .Select(x => new PreviewUserResponse()
                {
                    Id = x.id,
                    Name = x.FullName,
                    ProfileImage = FileStorageHelper.GetUrl(x.ProfileImage),
                }).SingleAsync();

            var privateRoomResponse = new PrivateRoomResponse()
            {
                Id = privateRoom.id,
                Contact = previewContact
            };

            return new Success<PrivateRoomResponse>(privateRoomResponse);
        }

        public async Task<ReadResponse<PagedListResponse<PrivateRoomResponse>>> GetListAsync(Guid loggedInUserId, PaginationRequest paginationRequest)
        {
            var privateRoomListQuery = _context.PrivateChats
                .Where(x => x.User1Id == loggedInUserId || x.User2Id == loggedInUserId)
                .Select(x => new PrivateRoomDto
                {
                    Id = x.id,
                    ContactId = x.User1Id == loggedInUserId ? x.User2Id : x.User1Id
                })
                .Select(d => new PrivateRoomResponse
                {
                    Id = d.Id,
                    Contact = new PreviewUserResponse
                    {
                        Id = d.ContactId,
                        Name = "",
                        ProfileImage = ""
                    }
                });

            var pagedResult = await PagedListResponse<PrivateRoomResponse>.CreateAsync(
                privateRoomListQuery,
                paginationRequest.PageSize,
                paginationRequest.CurrentPage
            );

            foreach (var privateRoomResponse in pagedResult.Items)
            {
                privateRoomResponse.Contact = await _context.Users
                    .Where(x => x.id == privateRoomResponse.Contact.Id)
                    .Select(x => new PreviewUserResponse()
                    {
                        Id = x.id,
                        Name = x.FullName,
                        ProfileImage = FileStorageHelper.GetUrl(x.ProfileImage)
                    }).SingleAsync();
            }

            return new Success<PagedListResponse<PrivateRoomResponse>>(pagedResult);
        }

        public async Task<ReadResponse<RoomLastSeenResponse>> GetLastSeenAsync(Guid loggedInUserId, Guid roomId, Guid userId)
        {
            var isUserInRoom = await _context.PrivateChats.IsAnyAsync(x =>
                (x.User1Id == loggedInUserId || x.User2Id == loggedInUserId) && x.id == roomId);

            if (!isUserInRoom)
            {
                return new AccessDenied();
            }

            var lastSeenResponse = await _context.RoomLastSeen.Select(x => new RoomLastSeenResponse()
            {
                UserId = x.UserId,
                RoomId = x.RoomId,
                LastSeenUtc = x.LastSeenUtc
            }).SingleOrDefaultAsync();

            if (lastSeenResponse == null)
            {
                return new NotFound();
            }

            return new Success<RoomLastSeenResponse>(lastSeenResponse);
        }
    }
}
