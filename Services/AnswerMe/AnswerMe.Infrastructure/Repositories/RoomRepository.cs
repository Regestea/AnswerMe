using AnswerMe.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnswerMe.Application.DTOs.Room;
using AnswerMe.Domain.Entities;
using Models.Shared.RepositoriesResponseTypes;
using Models.Shared.Requests.Room;
using Models.Shared.Responses.Shared;
using AnswerMe.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using OneOf.Types;
using AnswerMe.Infrastructure.Services;

namespace AnswerMe.Infrastructure.Repositories
{
    //public class RoomRepository : IRoomRepository
    //{
    //    private readonly AnswerMeDbContext _context;
    //    private readonly FileStorageService _fileStorageService;

    //    public RoomRepository(AnswerMeDbContext context, FileStorageService fileStorageService)
    //    {
    //        _context = context;
    //        _fileStorageService = fileStorageService;
    //    }

    //    public async Task<ReadResponse<bool>> ExistRoomAsync(Guid id)
    //    {
    //        var existRoom = await _context.Rooms.AnyAsync(x => x.id == id);

    //        return new Success<bool>(existRoom);
    //    }

    //    public async Task<CreateResponse<IdResponse>> AddAsync(AddRoomDto addRoomDto)
    //    {
    //        var room = new RoomChat()
    //        {
    //            id = Guid.NewGuid(),
    //            CreatedDate = DateTimeOffset.UtcNow,
    //            Name = addRoomDto.Name,
    //            Type = addRoomDto.RoomType
    //        };

    //        if (!string.IsNullOrWhiteSpace(addRoomDto.ImageToken))
    //        {
    //            var response = await _fileStorageService.GetObjectPath(addRoomDto.UserId, addRoomDto.ImageToken);
    //            if (!string.IsNullOrWhiteSpace(response.FilePath) && !string.IsNullOrWhiteSpace(response.FileType))
    //            {
    //                room.RoomImage = response.FilePath;
    //            }
    //        }

    //        await _context.Rooms.AddAsync(room);
    //        await _context.SaveChangesAsync();

    //        return new Success<IdResponse>(new IdResponse() { Id = room.id });
    //    }

    //    public async Task<UpdateResponse> EditAsync(Guid roomId, Guid userId, EditRoomRequest request)
    //    {
    //        var room = _context.Rooms.FirstOrDefault(x => x.id == roomId);

    //        if (room == null)
    //        {
    //            return new NotFound();
    //        }

    //        if (!string.IsNullOrWhiteSpace(request.ImageToken))
    //        {
    //            var response = await _fileStorageService.GetObjectPath(userId, request.ImageToken);
    //            if (!string.IsNullOrWhiteSpace(response.FilePath) && !string.IsNullOrWhiteSpace(response.FileType))
    //            {
    //                room.RoomImage = response.FilePath;
    //            }
    //        }

    //        _context.Rooms.Update(room);
    //        await _context.SaveChangesAsync();

    //        return new Success<Guid>(room.id);
    //    }
    //}
}
