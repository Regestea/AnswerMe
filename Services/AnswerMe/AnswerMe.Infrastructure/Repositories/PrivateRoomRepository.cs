using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnswerMe.Application.Common.Interfaces;
using AnswerMe.Application.DTOs.Room;
using AnswerMe.Application.DTOs.User;
using AnswerMe.Application.Extensions;
using AnswerMe.Domain.Entities;
using AnswerMe.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Models.Shared.OneOfTypes;
using Models.Shared.RepositoriesResponseTypes;
using Models.Shared.Requests.Shared;
using Models.Shared.Responses.Group;
using Models.Shared.Responses.Message;
using Models.Shared.Responses.PrivateRoom;
using Models.Shared.Responses.Shared;
using OneOf.Types;

namespace AnswerMe.Infrastructure.Repositories
{
    public class PrivateRoomRepository : IPrivateRoomRepository
    {
        private readonly AnswerMeDbContext _context;
        private ICacheRepository _cacheRepository;

        public PrivateRoomRepository(AnswerMeDbContext context, ICacheRepository cacheRepository)
        {
            _context = context;
            _cacheRepository = cacheRepository;
        }

        public async Task<ReadResponse<PrivateRoomResponse>> GetAsync(Guid loggedInUserId, Guid roomId)
        {
            var existRoom = await _context.PrivateChats.AnyAsync(x => x.id == roomId);

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

            var contactId = privateRoom.User1Id != loggedInUserId ? privateRoom.User1Id : privateRoom.User2Id;

            var isOnline = await _cacheRepository.GetAsync<UserOnlineDto>("Online-" + contactId) != null;

            var previewContact = await _context.Users
                .Where(x => x.id == contactId)
                .Select(x => new PreviewUserResponse()
                {
                    Id = x.id,
                    Name = x.FullName,
                    ProfileImage = FileStorageHelper.GetUrl(x.ProfileImage),
                    IsOnline = isOnline
                })
                .FirstOrDefaultAsync();


            var roomNotify = new RoomNotifyResponse() { TotalUnRead = 0, RoomId = roomId };

            var isInRoom = await _cacheRepository.GetAsync<RoomConnectionDto>(loggedInUserId.ToString());

            if (isInRoom != null)
            {
                var lastVisit = await _context.RoomLastSeen
                    .Where(x => x.RoomId == roomId && x.UserId == loggedInUserId)
                    .FirstOrDefaultAsync();
                if (lastVisit != null)
                {
                    roomNotify.TotalUnRead = await _context.Messages
                        .Where(x => x.RoomChatId == roomId && x.CreatedDate > lastVisit.LastSeenUtc)
                        .CountAsync();
                }
            }

            roomNotify.MessageGlance = await _context.Messages.Where(x => x.RoomChatId == roomId)
                .Select(x => x.Text.Substring(0, 10))
                .FirstOrDefaultAsync() ?? string.Empty;

            var privateRoomResponse = new PrivateRoomResponse()
            {
                RoomNotify = roomNotify,
                Contact = previewContact
            };

            return new Success<PrivateRoomResponse>(privateRoomResponse);
        }

        public async Task<ReadResponse<BooleanResponse>> IsOnlineInRoom(Guid loggedInUserId, Guid userId, Guid roomId)
        {
            var isInRoom = await _context.PrivateChats
                .AnyAsync(x => x.id == roomId && (x.User1Id == userId || x.User2Id == userId));

            if (!isInRoom)
            {
                return new Success<BooleanResponse>(
                    new BooleanResponse { FieldName = "IsOnlineInRoom", Result = false });
            }
    
            var roomConnection = await _cacheRepository.GetAsync<RoomConnectionDto>(userId.ToString());
    
            if (roomConnection?.RoomId == roomId)
            {
                return new Success<BooleanResponse>(
                    new BooleanResponse { FieldName = "IsOnlineInRoom", Result = true });
            }
    
            return new Success<BooleanResponse>(
                new BooleanResponse { FieldName = "IsOnlineInRoom", Result = false });
        }

        public async Task<CreateResponse<IdResponse>> CreateAsync(Guid loggedInUserId, Guid contactId)
        {
            var existContact = await _context.Users.AnyAsync(x => x.id == contactId);

            if (!existContact)
            {
                return new NotFound();
            }

            var privateChat = await _context.PrivateChats.Where(x =>
                    (x.User1Id == loggedInUserId && x.User2Id == contactId) ||
                    x.User2Id == loggedInUserId && x.User1Id == contactId)
                .SingleOrDefaultAsync();

            if (privateChat != null)
            {
                return new Success<IdResponse>(new IdResponse() { FieldName = "Private Room Id", Id = privateChat.id });
            }

            privateChat = new PrivateChat()
            {
                id = Guid.NewGuid(),
                User1Id = loggedInUserId,
                User2Id = contactId,
                CreatedDate = DateTimeOffset.UtcNow
            };

            await _context.PrivateChats.AddAsync(privateChat);
            await _context.SaveChangesAsync();

            return new Success<IdResponse>(new IdResponse()
            {
                Id = privateChat.id,
                FieldName = "Private Room Id"
            });
        }

        public async Task<ReadResponse<PagedListResponse<PrivateRoomResponse>>> GetListAsync(Guid loggedInUserId,
            PaginationRequest paginationRequest)
        {
            var privateRoomListQuery = _context.PrivateChats
                .Where(x => x.User1Id == loggedInUserId || x.User2Id == loggedInUserId)
                .OrderByDescending(x=>x.CreatedDate)
                .Select(x => new PrivateRoomDto
                {
                    Id = x.id,
                    ContactId = x.User1Id == loggedInUserId ? x.User2Id : x.User1Id
                })
                .Select(d => new PrivateRoomResponse
                {
                    RoomNotify = new RoomNotifyResponse()
                    {
                        RoomId = d.Id,
                        TotalUnRead = 0,
                        MessageGlance = ""
                    },
                    Contact = new PreviewUserResponse
                    {
                        Id = d.ContactId,
                        Name = "",
                        ProfileImage = ""
                    }
                });
            
            var pagedResult = await PagedListResponse<PrivateRoomResponse>.CreateAsync(
                privateRoomListQuery,
                paginationRequest
            );

            
            foreach (var privateRoomResponse in pagedResult.Items)
            {
                var isOnline= await _cacheRepository.GetAsync<UserOnlineDto>("Online-"+privateRoomResponse.Contact.Id) != null;
                
                privateRoomResponse.Contact = await _context.Users
                    .Where(x => x.id == privateRoomResponse.Contact.Id)
                    .Select(x => new PreviewUserResponse()
                    {
                        Id = x.id,
                        Name = x.FullName,
                        ProfileImage = FileStorageHelper.GetUrl(x.ProfileImage),
                        IsOnline = isOnline
                    }).SingleAsync();
                
                var isInRoom = await _cacheRepository.GetAsync<RoomConnectionDto>(loggedInUserId.ToString());
                
                if (isInRoom == null)
                {
                    var lastVisit = await _context.RoomLastSeen
                        .Where(x => x.RoomId == privateRoomResponse.RoomNotify.RoomId && x.UserId == loggedInUserId)
                        .FirstOrDefaultAsync();
                    if (lastVisit != null)
                    {
                        privateRoomResponse.RoomNotify.TotalUnRead = await _context.Messages
                            .Where(x => x.RoomChatId == privateRoomResponse.RoomNotify.RoomId && x.CreatedDate > lastVisit.LastSeenUtc)
                            .CountAsync();
                    }
                }

                privateRoomResponse.RoomNotify.MessageGlance = await _context.Messages
                    .Where(x => x.RoomChatId == privateRoomResponse.RoomNotify.RoomId)
                    .OrderByDescending(x=>x.CreatedDate)
                    .Select(x => x.Text)
                    .FirstOrDefaultAsync() ?? string.Empty;
                
              
            }

            return new Success<PagedListResponse<PrivateRoomResponse>>(pagedResult);
        }

        public async Task<ReadResponse<RoomLastSeenResponse>> GetLastSeenAsync(Guid loggedInUserId, Guid roomId,
            Guid userId)
        {
            var isUserInRoom = await _context.PrivateChats.AnyAsync(x =>
                (x.User1Id == loggedInUserId || x.User2Id == loggedInUserId) && x.id == roomId);

            if (!isUserInRoom)
            {
                return new NotFound();
            }

            var lastSeenResponse = await _context.RoomLastSeen
                .Where(x=>x.RoomId == roomId&&x.UserId == userId)
                .Select(x => new RoomLastSeenResponse()
            {
                UserId = x.UserId,
                RoomId = x.RoomId,
                LastSeenUtc = x.LastSeenUtc
            }).FirstOrDefaultAsync();

            if (lastSeenResponse == null)
            {
                return new NotFound();
            }

            return new Success<RoomLastSeenResponse>(lastSeenResponse);
        }
    }
}