using AnswerMe.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnswerMe.Application.Extensions;
using AnswerMe.Domain.Entities;
using Models.Shared.RepositoriesResponseTypes;
using Models.Shared.Responses.Shared;
using AnswerMe.Infrastructure.Persistence;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.EntityFrameworkCore;
using OneOf.Types;

namespace AnswerMe.Infrastructure.Repositories
{
    public class GroupRoomAdminRepository : IGroupRoomAdminRepository
    {
        private readonly AnswerMeDbContext _context;

        public GroupRoomAdminRepository(AnswerMeDbContext context)
        {
            _context = context;
        }


        public async Task<ReadResponse<bool>> IsAdminAsync(Guid userId, Guid roomId)
        {
            var isAdmin = await _context.Users.IsAnyAsync(x=>true);
                //.CountAsync(x=>x.id==roomId && x.RoomAdminIds.Equals(new IdEntity() { Id = userId })) >= 1;
                //.IsAnyAsync(x => x.id == roomId && x.RoomAdminIds.Contains(new IdEntity(){Id = userId}));

            return new Success<bool>(true);
        }

        public Task<ReadResponse<List<IdResponse>>> AdminListAsync(Guid roomId)
        {
            throw new NotImplementedException();
        }

        public Task<CreateResponse<IdResponse>> AddAsync(Guid userId, Guid roomId)
        {
            throw new NotImplementedException();
        }

        public Task<DeleteResponse> DeleteAsync(Guid userId, Guid roomId)
        {
            throw new NotImplementedException();
        }

        //public async Task<ReadResponse<List<IdResponse>>> AdminListAsync(Guid roomId)
        //{
        //    var adminListId = await _context.RoomAdmins.Where(x => x.RoomId == roomId)
        //        .Select(x => new IdResponse() { Id = x.UserId })
        //        .ToListAsync();

        //    return new Success<List<IdResponse>>(adminListId);
        //}

        //public async Task<CreateResponse<IdResponse>> AddAsync(Guid userId, Guid roomId)
        //{
        //    var existUser = await _context.Users.AnyAsync(x => x.id == userId);
        //    var existRoom = await _context.Rooms.AnyAsync(x => x.id == roomId);

        //    if (existRoom == false || existUser == false)
        //    {
        //        return new Error<string>("user or room doesn't exist");
        //    }

        //    var roomAdmin = new RoomAdmin()
        //    {
        //        id = Guid.NewGuid(),
        //        UserId = userId,
        //        RoomId = roomId,
        //        CreatedDate = DateTimeOffset.UtcNow
        //    };

        //    await _context.RoomAdmins.AddAsync(roomAdmin);
        //    await _context.SaveChangesAsync();

        //    return new Success<IdResponse>(new IdResponse() { Id = roomAdmin.id });
        //}

        //public async Task<DeleteResponse> DeleteAsync(Guid userId, Guid roomId)
        //{
        //    var roomAdmin = _context.RoomAdmins.FirstOrDefault(x => x.id == userId && x.RoomId == roomId);
        //    if (roomAdmin != null)
        //    {
        //        _context.RoomAdmins.Remove(roomAdmin);
        //        await _context.SaveChangesAsync();
        //    }

        //    return new Success();
        //}
    }
}
