using AnswerMe.Application.Common.Interfaces;
using AnswerMe.Application.DTOs;
using AnswerMe.Application.Extensions;
using AnswerMe.Domain.Entities;
using AnswerMe.Infrastructure.Persistence;
using Models.Shared.Requests.User;
using Models.Shared.Responses.Shared;
using OneOf.Types;
using AnswerMe.Application.DTOs.User;
using AnswerMe.Infrastructure.Services;
using Models.Shared.DTOs.Error;
using Models.Shared.RepositoriesResponseTypes;

using Microsoft.EntityFrameworkCore;
using Models.Shared.Requests.Shared;
using Models.Shared.Responses.User;

namespace AnswerMe.Infrastructure.Repositories
{
    //public class UserRepository : IUserRepository
    //{
    //    private readonly AnswerMeDbContext _context;
    //    private readonly FileStorageService _fileStorageService;

    //    public UserRepository(AnswerMeDbContext context, FileStorageService fileStorageService)
    //    {
    //        _context = context;
    //        _fileStorageService = fileStorageService;
    //    }

    //     save blur hash at answer me api side into media entity 
    //    public async Task<ReadResponse<bool>> ExistUserAsync(Guid id)
    //    {
    //        var response= await _context.Users.AnyAsync(x => x.id == id);
    //        return new Success<bool>(response);
    //    }

    //    public async Task<CreateResponse<IdResponse>> AddUserAsync(AddUserDto userDto)
    //    {
    //        var user = new User
    //        {
    //            id = userDto.id,
    //            FullName = userDto.IdName,
    //            IdName = userDto.IdName,
    //            PhoneNumber = userDto.PhoneNumber,
    //        };
    //        await _context.Users.AddAsync(user);
    //        await _context.SaveChangesAsync();
    //        return new Success<IdResponse>(new IdResponse() { Id = user.id });
    //    }

    //    public async Task<ReadResponse<UserResponse>> GetUserByIdAsync(Guid id)
    //    {
    //        var userDto = await _context.Users.Where(x => x.id == id)
    //            .Select(x => new UserResponse()
    //            {
    //                id = x.id,
    //                PhoneNumber = x.PhoneNumber,
    //                IdName = x.IdName,
    //                CreatedDate = x.CreatedDate,
    //                FullName = x.FullName,
    //                ModifiedDate = x.ModifiedDate,
    //                ProfileImage = x.ProfileImage
    //            })
    //            .SingleOrDefaultAsync();

    //        if (userDto != null)
    //        {
    //            return new Success<UserResponse>(userDto);
    //        }

    //        return new NotFound();
    //    }

    //    public async Task<UpdateResponse> EditUserAsync(Guid id, EditUserRequest request)
    //    {
    //        var user = await _context.Users.SingleAsync(x => x.id == id);

    //        if (!string.IsNullOrWhiteSpace(request.FullName))
    //        {
    //            user.FullName=request.FullName;
    //        }

    //        if (!string.IsNullOrWhiteSpace(request.ProfileImageToken))
    //        {
    //            var response = await _fileStorageService.GetObjectPath(id, request.ProfileImageToken);
    //            if (!string.IsNullOrWhiteSpace(response.FilePath) && !string.IsNullOrWhiteSpace(response.FileType))
    //            {
    //                user.ProfileImage = response.FilePath;
    //            }
    //        }

    //        _context.Users.Update(user);

    //        await _context.SaveChangesAsync();

    //        return new Success<Guid>(user.id);
    //    }
    //}
}
