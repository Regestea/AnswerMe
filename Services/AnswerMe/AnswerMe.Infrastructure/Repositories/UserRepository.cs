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
using Models.Shared.RepositoriesResponseTypes;
using Microsoft.EntityFrameworkCore;
using Models.Shared.Requests.Shared;
using Models.Shared.Responses.User;

namespace AnswerMe.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AnswerMeDbContext _context;
        private readonly FileStorageService _fileStorageService;
        private readonly IOnlineHubService _onlineHubService;

        public UserRepository(AnswerMeDbContext context, FileStorageService fileStorageService,
            IOnlineHubService onlineHubService)
        {
            _context = context;
            _fileStorageService = fileStorageService;
            _onlineHubService = onlineHubService;
        }


        public async Task<ReadResponse<BooleanResponse>> IsOnlineAsync(Guid id)
        {
            var isOnline = await _onlineHubService.IsOnlineAsync(id);

            return new Success<BooleanResponse>(isOnline.AsT0.Value);
        }

        public async Task<ReadResponse<PagedListResponse<UserResponse>>> SearchUserAsync(Guid loggedInUserId,string keyWord,
            PaginationRequest paginationRequest)
        {
            var userQuery = _context.Users
                .Where(x=>x.id != loggedInUserId)
                .OrderByDescending(x=>x.CreatedDate)
                .Where(x => x.PhoneNumber
                    .Contains(keyWord) || x.IdName
                    .Contains(keyWord.ToLower()))
                .Select(x => new UserResponse()
                {
                    id = x.id,
                    FullName = x.FullName,
                    IdName = x.IdName,
                    CreatedDate = x.CreatedDate,
                    ModifiedDate = x.ModifiedDate,
                    PhoneNumber = x.PhoneNumber,
                    ProfileImage = FileStorageHelper.GetUrl(x.ProfileImage)
                })
                .AsQueryable();

            var pagedResult = await PagedListResponse<UserResponse>.CreateAsync(
                userQuery,
                paginationRequest
            );

            return new Success<PagedListResponse<UserResponse>>(pagedResult);
        }

        public async Task<ReadResponse<BooleanResponse>> ExistAsync(Guid id)
        {
            var response = await _context.Users.AnyAsync(x => x.id == id);

            return new Success<BooleanResponse>(new BooleanResponse() { FieldName = "ExistUser", Result = response });
        }

        public async Task<CreateResponse<IdResponse>> AddAsync(AddUserDto userDto)
        {
            var user = new User
            {
                id = userDto.id,
                FullName = userDto.IdName,
                IdName = userDto.IdName,
                PhoneNumber = userDto.PhoneNumber,
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return new Success<IdResponse>(new IdResponse() { FieldName = "UserId", Id = user.id });
        }

        public async Task<ReadResponse<UserResponse>> GetByIdAsync(Guid id)
        {
            var userDto = await _context.Users.Where(x => x.id == id)
                .Select(x => new UserResponse()
                {
                    id = x.id,
                    PhoneNumber = x.PhoneNumber,
                    IdName = x.IdName,
                    CreatedDate = x.CreatedDate,
                    FullName = x.FullName,
                    ModifiedDate = x.ModifiedDate,
                    ProfileImage = FileStorageHelper.GetUrl(x.ProfileImage)
                })
                .FirstOrDefaultAsync();

            if (userDto != null)
            {
                return new Success<UserResponse>(userDto);
            }

            return new NotFound();
        }

        public async Task<UpdateResponse> EditAsync(Guid loggedInUserId, EditUserRequest request)
        {
            var user = await _context.Users.SingleAsync(x => x.id == loggedInUserId);

            if (!string.IsNullOrWhiteSpace(request.FullName))
            {
                user.FullName = request.FullName;
            }

            var oldProfileImage = user.ProfileImage;
            if (!string.IsNullOrWhiteSpace(request.ProfileImageToken))
            {
                var response = await _fileStorageService.GetObjectPathAsync(loggedInUserId, request.ProfileImageToken);
                if (!string.IsNullOrWhiteSpace(response.FilePath) && !string.IsNullOrWhiteSpace(response.FileFormat))
                {
                    user.ProfileImage = response.FilePath;
                    if (!string.IsNullOrWhiteSpace(oldProfileImage))
                    {
                        Console.WriteLine("try to remove " + oldProfileImage);
                        await _fileStorageService.DeleteObjectAsync(loggedInUserId, oldProfileImage);
                    }
                }
            }
            user.ModifiedDate=DateTimeOffset.UtcNow;

            _context.Users.Update(user);

            await _context.SaveChangesAsync();

            return new Success();
        }
    }
}