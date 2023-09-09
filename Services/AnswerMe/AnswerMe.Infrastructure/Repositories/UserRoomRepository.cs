using AnswerMe.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnswerMe.Domain.Entities;
using Models.Shared.RepositoriesResponseTypes;
using Models.Shared.Responses.Shared;
using AnswerMe.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Models.Shared.DTOs.Error;
using OneOf.Types;

namespace AnswerMe.Infrastructure.Repositories
{
    //public class UserRoomRepository : IUserRoomRepository
    //{
    //    private readonly AnswerMeDbContext _context;

    //    public UserRoomRepository(AnswerMeDbContext context)
    //    {
    //        _context = context;
    //    }

    //    public async Task<CreateResponse<IdResponse>> AddAsync(Guid userId, Guid roomId)
    //    {
    //        var existRoom = await _context.Rooms.AnyAsync(x => x.id == roomId);
    //        if (!existRoom)
    //        {
    //            var errors = new List<ValidationFailedDto>();

    //            if (existRoom == false)
    //            {
    //                errors.Add(new ValidationFailedDto() { Field = nameof(userId), Error = "this room doesn't exist" });
    //            }

    //            return errors;
    //        }

    //        var userRoom = new UserRoom()
    //        {
    //            id = Guid.NewGuid(),
    //            CreatedDate = DateTimeOffset.UtcNow,
    //            UserId = userId,
    //            RoomId = roomId
    //        };

    //        await _context.UserRooms.AddAsync(userRoom);
    //        await _context.SaveChangesAsync();

    //        return new Success<IdResponse>(new IdResponse() { Id = userRoom.id });
    //    }

    //    public async Task<DeleteResponse> DeleteAsync(Guid userId, Guid roomId)
    //    {
    //        var userRoom = await _context.UserRooms.FirstOrDefaultAsync(x => x.UserId == userId && x.RoomId == roomId);
    //        if (userRoom != null)
    //        {
    //            _context.UserRooms.Remove(userRoom);

    //            await _context.SaveChangesAsync();
    //        }

    //        return new Success();
    //    }

    //    public async Task<ReadResponse<List<IdResponse>>> UserRoomListAsync(Guid userId)
    //    {
    //        var userRoomList = await _context.UserRooms.Where(x => x.UserId == userId)
    //            .Select(x => new IdResponse()
    //            {
    //                Id = x.RoomId
    //            })
    //            .ToListAsync();

    //        return new Success<List<IdResponse>>(userRoomList);
    //    }

    //    public async Task<ReadResponse<List<IdResponse>>> RoomUserListAsync(Guid roomId)
    //    {
    //        var roomUserList = await _context.UserRooms.Where(x => x.RoomId == roomId)
    //            .Select(x => new IdResponse()
    //            {
    //                Id = x.UserId
    //            })
    //            .ToListAsync();

    //        return new Success<List<IdResponse>>(roomUserList);
    //    }
    //}
}
