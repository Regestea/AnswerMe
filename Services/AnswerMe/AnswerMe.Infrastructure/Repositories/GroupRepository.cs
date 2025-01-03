﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnswerMe.Application.Extensions;
using AnswerMe.Domain.Entities;
using AnswerMe.Infrastructure.Persistence;
using AnswerMe.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Models.Shared.RepositoriesResponseTypes;
using Models.Shared.Requests.Shared;
using Models.Shared.Responses.Group;
using Models.Shared.Responses.Shared;
using OneOf.Types;
using Models.Shared.OneOfTypes;
using Models.Shared.Requests.Group;
using AnswerMe.Application.Common.Interfaces;
using AnswerMe.Application.DTOs.Room;
using Google.Protobuf;
using Models.Shared.Responses.Message;
using Models.Shared.Responses.PrivateRoom;
using Models.Shared.Responses.User;

namespace AnswerMe.Infrastructure.Repositories
{
    public class GroupRepository : IGroupRepository
    {
        private readonly AnswerMeDbContext _context;
        private readonly FileStorageService _fileStorageService;
        private ICacheRepository _cacheRepository;

        public GroupRepository(AnswerMeDbContext context, FileStorageService fileStorageService,
            ICacheRepository cacheRepository)
        {
            _context = context;
            _fileStorageService = fileStorageService;
            _cacheRepository = cacheRepository;
        }

        public async Task<ReadResponse<PreviewGroupResponse>> GetAsync(Guid loggedInUserId, Guid groupId)
        {
            var existGroup = await _context.GroupChats.AnyAsync(x => x.id == groupId);

            if (!existGroup)
            {
                return new NotFound();
            }

            var isUserInGroup =
                await _context.UserGroups.AnyAsync(x => x.UserId == loggedInUserId && x.GroupId == groupId);

            if (!isUserInGroup)
            {
                return new AccessDenied();
            }

            var groupResponse = await _context
                .GroupChats.Where(x => x.id == groupId)
                .Select(x => new PreviewGroupResponse()
                {
                    Id = x.id,
                    Name = x.Name,
                    CreatedDate = x.CreatedDate,
                    ModifiedDate = x.ModifiedDate,
                    RoomImage = FileStorageHelper.GetUrl(x.RoomImage)
                }).FirstOrDefaultAsync();

            if (groupResponse == null)
            {
                return new NotFound();
            }

            return new Success<PreviewGroupResponse>(groupResponse);
        }

        public async Task<ReadResponse<PagedListResponse<GroupResponse>>> GetListAsync(Guid loggedInUserId,
            PaginationRequest paginationRequest)
        {
            var groupIdList = await _context.UserGroups
                .OrderByDescending(x => x.CreatedDate)
                .Where(x => x.UserId == loggedInUserId)
                .Select(x => new Guid(x.GroupId.ToString()))
                .ToListAsync();

            var groupQuery = _context.GroupChats.Where(x => groupIdList.Contains(x.id))
                .OrderByDescending(x => x.CreatedDate)
                .Select(x => new GroupResponse
                {
                    Group = new PreviewGroupResponse()
                    {
                        Id = x.id,
                        Name = x.Name,
                        CreatedDate = x.CreatedDate,
                        ModifiedDate = x.ModifiedDate,
                        RoomImage = FileStorageHelper.GetUrl(x.RoomImage)
                    },
                    RoomNotify = new RoomNotifyResponse()
                    {
                        RoomId = x.id,
                        MessageGlance = "",
                        TotalUnRead = 0,
                    }
                });

            var pagedResult = await PagedListResponse<GroupResponse>.CreateAsync(
                groupQuery,
                paginationRequest
            );

            foreach (var groupResponse in pagedResult.Items)
            {
                var isInRoom = await _cacheRepository.GetAsync<RoomConnectionDto>("GR-" + loggedInUserId.ToString());

                if (isInRoom == null)
                {
                    var lastVisit = await _context.RoomLastSeen
                        .Where(x => x.RoomId == groupResponse.RoomNotify.RoomId && x.UserId == loggedInUserId)
                        .FirstOrDefaultAsync();
                    if (lastVisit != null)
                    {
                        groupResponse.RoomNotify.TotalUnRead = await _context.Messages
                            .Where(x => x.RoomChatId == groupResponse.RoomNotify.RoomId &&
                                        x.CreatedDate > lastVisit.LastSeenUtc)
                            .CountAsync();
                    }
                }

                groupResponse.RoomNotify.MessageGlance = await _context.Messages
                    .Where(x => x.RoomChatId == groupResponse.RoomNotify.RoomId)
                    .OrderByDescending(x => x.CreatedDate)
                    .Select(x => x.Text)
                    .FirstOrDefaultAsync() ?? string.Empty;
            }

            return new Success<PagedListResponse<GroupResponse>>(pagedResult);
        }

        public async Task<ReadResponse<PagedListResponse<PreviewGroupUserResponse>>> UserListAsync(Guid loggedInUserId,
            Guid groupId, PaginationRequest paginationRequest)
        {
            var existGroup = await _context.GroupChats.AnyAsync(x => x.id == groupId);

            if (!existGroup)
            {
                return new NotFound();
            }

            var isUserInGroup =
                await _context.UserGroups.AnyAsync(x => x.UserId == loggedInUserId && x.GroupId == groupId);

            if (!isUserInGroup)
            {
                return new AccessDenied();
            }

            var groupUserIdList = await _context.UserGroups
                .Where(x => x.GroupId == groupId&& x.UserId != loggedInUserId)
                .OrderByDescending(x => x.CreatedDate)
                .Select(x => x.UserId)
                .ToListAsync();


            var previewUserQuery = _context.Users
                .Where(x => groupUserIdList.Contains(x.id))
                .Select(x => new PreviewGroupUserResponse()
                {
                    Id = x.id,
                    Name = x.FullName,
                    ProfileImage = FileStorageHelper.GetUrl(x.ProfileImage),
                    IsAdmin = false
                });

            var pagedResult = await PagedListResponse<PreviewGroupUserResponse>.CreateAsync(
                previewUserQuery,
                paginationRequest
            );

            var adminIdList = await _context.GroupAdmins
                .Where(x => pagedResult.Items.Select(x => x.Id).Contains(x.UserId) && x.RoomId == groupId)
                .Select(x => x.UserId)
                .ToListAsync();

            pagedResult.Items = pagedResult.Items.Select(item =>
            {
                if (adminIdList.Contains(item.Id))
                {
                    item.IsAdmin = true;
                }

                return item;
            }).ToList();

            return new Success<PagedListResponse<PreviewGroupUserResponse>>(pagedResult);
        }

        public async Task<CreateResponse<IdResponse>> SetUserAsAdminAsync(Guid loggedInUserId, Guid groupId,
            Guid userId)
        {
            var isAdmin = await _context.GroupAdmins.AnyAsync(x => x.UserId == loggedInUserId && x.RoomId == groupId);

            if (!isAdmin)
            {
                return new AccessDenied();
            }

            var existUser = await _context.Users.AnyAsync(x => x.id == userId);

            if (!existUser)
            {
                return new NotFound();
            }

            var existGroup = await _context.GroupChats.AnyAsync(x => x.id == groupId);

            if (!existGroup)
            {
                return new NotFound();
            }

            await _context.GroupAdmins.AddAsync(new GroupAdmin() { UserId = userId, RoomId = groupId });

            await _context.SaveChangesAsync();

            return new Success<IdResponse>(new IdResponse() { FieldName = "UserId", Id = userId });
        }

        public async Task<ReadResponse<BooleanResponse>> IsAdminAsync(Guid loggedInUserId, Guid groupId, Guid userId)
        {
            var isInGroup=await _context.UserGroups.AnyAsync(x=>x.UserId==userId&&x.GroupId==groupId);
            if (isInGroup==false)
            {
                return new AccessDenied();
            }

            var isAdmin = await _context.GroupAdmins.AnyAsync(x => x.UserId == userId && x.RoomId == groupId);
            
            return new Success<BooleanResponse>(new BooleanResponse(){FieldName = "Is Admin", Result = isAdmin});
        }

        public async Task<DeleteResponse> RemoveUserFromAdminsAsync(Guid loggedInUserId, Guid groupId, Guid userId)
        {
            var existUser = await _context.Users.AnyAsync(x => x.id == userId);

            if (!existUser)
            {
                return new NotFound();
            }

            var existGroup = await _context.GroupChats.AnyAsync(x => x.id == groupId);

            if (!existGroup)
            {
                return new NotFound();
            }

            var isAdmin = await _context.GroupAdmins.AnyAsync(x => x.UserId == loggedInUserId && x.RoomId == groupId);

            if (isAdmin)
            {
                var admin = await _context.GroupAdmins.FirstOrDefaultAsync(x =>
                    x.UserId == userId && x.RoomId == groupId);

                if (admin != null)
                {
                    _context.GroupAdmins.Remove(admin);

                    await _context.SaveChangesAsync();

                    return new Success();
                }
            }

            return new AccessDenied();
        }

        public async Task<CreateResponse<IdResponse>> CreateAsync(Guid loggedInUserId, CreateGroupRequest request)
        {
            var group = new GroupChat()
            {
                id = Guid.NewGuid(),
                CreatedDate = DateTimeOffset.UtcNow,
                Name = request.Name,
            };

            if (!string.IsNullOrWhiteSpace(request.ImageToken))
            {
                var response = await _fileStorageService.GetObjectPathAsync(loggedInUserId, request.ImageToken);
                if (!string.IsNullOrWhiteSpace(response.FilePath))
                {
                    group.RoomImage = response.FilePath;
                }
            }

            await _context.GroupChats.AddAsync(group);
            await _context.GroupAdmins.AddAsync(new GroupAdmin() { UserId = loggedInUserId, RoomId = group.id });
            await _context.UserGroups.AddAsync(new UserGroup() { UserId = loggedInUserId, GroupId = group.id });

            await _context.SaveChangesAsync();

            return new Success<IdResponse>(new IdResponse() { FieldName = "GroupId", Id = group.id });
        }

        public async Task<UpdateResponse> EditAsync(Guid loggedInUserId, Guid groupId, EditGroupRequest request)
        {
            var existGroup = await _context.GroupChats.AnyAsync(x => x.id == groupId);

            if (!existGroup)
            {
                return new NotFound();
            }

            var isAdmin = await _context.GroupAdmins.AnyAsync(x => x.UserId == loggedInUserId && x.RoomId == groupId);

            if (!isAdmin)
            {
                return new AccessDenied();
            }

            var group = await _context.GroupChats.FirstOrDefaultAsync(x => x.id == groupId);

            if (group == null)
            {
                return new NotFound();
            }

            group.Name = request.Name;

            if (!string.IsNullOrWhiteSpace(request.ImageToken))
            {
                var response = await _fileStorageService.GetObjectPathAsync(loggedInUserId, request.ImageToken);
                if (!string.IsNullOrWhiteSpace(response.FilePath))
                {
                    if (!string.IsNullOrWhiteSpace(group.RoomImage))
                    {
                        await _fileStorageService.DeleteObjectAsync(loggedInUserId, group.RoomImage);
                    }

                    group.RoomImage = response.FilePath;
                }
            }

            _context.GroupChats.Update(group);
            await _context.SaveChangesAsync();

            return new Success();
        }

        public async Task<ReadResponse<RoomLastSeenResponse>> GetLastSeenAsync(Guid loggedInUserId, Guid groupId, Guid userId)
        {
            var isUserInGroup = await _context.UserGroups.AnyAsync(x =>
                x.UserId == loggedInUserId && x.GroupId == groupId);

            if (!isUserInGroup)
            {
                return new NotFound();
            }

            var lastSeenResponse = await _context.RoomLastSeen
                .Where(x => x.RoomId == groupId && x.UserId == userId)
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

        public async Task<ReadResponse<PagedListResponse<UserResponse>>> GetUnAddedContactsAsync(Guid loggedInUserId, Guid groupId, PaginationRequest paginationRequest)
        {
            var existGroup =await _context.GroupChats.AnyAsync(x => x.id == groupId);

            if (!existGroup)
            {
                return new NotFound();
            }
            
            var isUserInGroup =await _context.UserGroups.AnyAsync(x => x.GroupId == groupId && x.UserId == loggedInUserId);
            if (!isUserInGroup)
            {
                return new AccessDenied();
            }
            
            var contactIdList=await _context.PrivateChats.Where(x=>x.User1Id==loggedInUserId||x.User2Id==loggedInUserId)
                .OrderByDescending(x=>x.CreatedDate)
                .Select(x=>x.User1Id==loggedInUserId?x.User2Id:x.User1Id)
                .Where(x=>_context.UserGroups.Any(y=>y.UserId==x && y.GroupId==groupId)==false)
                .ToListAsync();

            var usersQuery =  _context.Users
                .Where(x => contactIdList.Contains(x.id))
                .Select(x => new UserResponse()
                {
                    id = x.id,
                    FullName = x.FullName,
                    IdName = x.IdName,
                    PhoneNumber = x.PhoneNumber,
                    ProfileImage = FileStorageHelper.GetUrl(x.ProfileImage),
                    CreatedDate = x.CreatedDate,
                    ModifiedDate = x.ModifiedDate
                });
            var pagedUsers =await PagedListResponse<UserResponse>.CreateAsync(usersQuery, paginationRequest);
            
            return new Success<PagedListResponse<UserResponse>>(pagedUsers);
            
            
        }

        public async Task<CreateResponse<IdResponse>> JoinUserAsync(Guid loggedInUserId, Guid groupId, Guid joinUserId)
        {
            var existUser = await _context.Users.AnyAsync(x => x.id == joinUserId);

            if (!existUser)
            {
                return new NotFound();
            }

            var existGroup = await _context.GroupChats.AnyAsync(x => x.id == groupId);

            if (!existGroup)
            {
                return new NotFound();
            }

            var isAdmin = await _context.GroupAdmins.AnyAsync(x => x.UserId == loggedInUserId && x.RoomId == groupId);

            if (!isAdmin)
            {
                return new AccessDenied();
            }


            await _context.UserGroups.AddAsync(new UserGroup()
            {
                UserId = joinUserId,
                GroupId = groupId
            });
            await _context.SaveChangesAsync();

            return new Success<IdResponse>(new IdResponse() { FieldName = "JoinedUserId", Id = joinUserId });
        }

        public async Task<ReadResponse<MemberCountResponse>> MembersCountAsync(Guid loggedInUserId, Guid groupId)
        {
            var isInGroup=await _context.UserGroups.AnyAsync(x=>x.UserId==loggedInUserId && x.GroupId==groupId);

            if (!isInGroup)
            {
                return new AccessDenied();
            }

            var result = await _context.UserGroups
                .Where(x => x.GroupId == groupId)
                .CountAsync();
            
            return new Success<MemberCountResponse>(new MemberCountResponse(){Count = result});
            
        }

        public async Task<DeleteResponse> KickUserAsync(Guid loggedInUserId, Guid groupId, Guid kickUserId)
        {
            var isAdmin = await _context.GroupAdmins.AnyAsync(x => x.UserId == loggedInUserId && x.RoomId == groupId);

            if (isAdmin)
            {
                var userGroup =
                    await _context.UserGroups.FirstOrDefaultAsync(x => x.UserId == kickUserId && x.GroupId == groupId);

                if (userGroup != null)
                {
                    _context.UserGroups.Remove(userGroup);
                    await _context.SaveChangesAsync();

                    return new Success();
                }

                return new NotFound();
            }

            return new AccessDenied();
        }

        public async Task<DeleteResponse> LeaveGroupAsync(Guid loggedInUserId, Guid groupId)
        {
            var userGroup =
                await _context.UserGroups.FirstOrDefaultAsync(x => x.UserId == loggedInUserId && x.GroupId == groupId);

            if (userGroup != null)
            {
                _context.UserGroups.Remove(userGroup);
                await _context.SaveChangesAsync();

                return new Success();
            }

            return new NotFound();
        }
    }
}