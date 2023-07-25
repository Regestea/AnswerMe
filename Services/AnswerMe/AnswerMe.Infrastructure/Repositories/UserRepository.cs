using AnswerMe.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnswerMe.Application.Extensions;
using AnswerMe.Application.RepositoriesResponseTypes;
using AnswerMe.Domain.Entities;
using AnswerMe.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Models.Shared.Requests.User;
using Models.Shared.Responses.Shared;
using OneOf.Types;
using AnswerMe.Application.DTOs.Error;
using AnswerMe.Application.DTOs.User;

namespace AnswerMe.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private AnswerMeDbContext _context;

        public UserRepository(AnswerMeDbContext context)
        {
            _context = context;
        }

        public async Task<CreateResponse<IdResponse>> AddUser(RegisterUserRequest request)
        {

            var existPhone =await _context.Users.Where(x => x.PhoneNumber == request.PhoneNumber.NormalizeString()).CountAsync() > 0;
            var existIdName =await _context.Users.Where(x => x.PhoneNumber == request.IdName.NormalizeString()).CountAsync() > 0;


            if (existPhone || existIdName)
            {
                var validationErrorList = new List<ValidationFailedDto>();
                if (existIdName)
                {
                    validationErrorList.Add(new ValidationFailedDto()
                    {
                        Field = nameof(request.IdName),
                        Error = $"this {nameof(request.IdName)} is taken try different {nameof(request.IdName)}"
                    });
                }

                if (existPhone)
                {
                    validationErrorList.Add(new ValidationFailedDto()
                    {
                        Field = nameof(request.PhoneNumber),
                        Error = $"this {nameof(request.PhoneNumber)} is taken try different {nameof(request.PhoneNumber)}"
                    });
                }

                return validationErrorList;
            }

            var user = new User
            {
                id = Guid.NewGuid(),
                FullName = request.FullName,
                IdName = request.IdName.NormalizeString(),
                Password = PasswordHash.Hash(request.Password),
                PhoneNumber = request.PhoneNumber.NormalizeString(),
            };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return new Success<IdResponse>(new IdResponse() { Id = user.id });
        }

        public async Task<ReadResponse<UserDto>> GetUser(LoginUserRequest request)
        {
            var userDto = await _context.Users.Where(x =>
                x.PhoneNumber == request.PhoneNumber.Normalize() && x.Password == PasswordHash.Hash(request.Password))
                .Select(x => new UserDto()
                {
                    id = x.id,
                    PhoneNumber = x.PhoneNumber,
                    IdName = x.IdName,
                    CreatedDate = x.CreatedDate,
                    FullName = x.FullName,
                    ModifiedDate = x.ModifiedDate,
                    ProfileImage = x.ProfileImage
                })
                .SingleOrDefaultAsync();

            if (userDto != null)
            {
                return new Success<UserDto>(userDto);
            }

            return new NotFound();
        }

        public Task<UpdateResponse> EditProfileImage(EditProfileImageRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task<ReadResponse<bool>> ExistPhone(string phone)
        {
            var exist = await _context.Users.AnyAsync(x => x.PhoneNumber == phone.NormalizeString());

            return new Success<bool>(exist);
        }

        public async Task<ReadResponse<bool>> ExistIdName(string idName)
        {
            var exist = await _context.Users.AnyAsync(x => x.IdName == idName.NormalizeString());

            return new Success<bool>(exist);
        }
    }
}
