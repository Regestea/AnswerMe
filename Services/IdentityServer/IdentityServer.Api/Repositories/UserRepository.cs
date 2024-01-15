using IdentityServer.Api.Context;
using IdentityServer.Api.DTOs.User;
using IdentityServer.Api.Entities;
using IdentityServer.Api.Extensions;
using Microsoft.EntityFrameworkCore;
using Models.Shared.OneOfTypes;
using Models.Shared.RepositoriesResponseTypes;
using Models.Shared.Requests.User;
using Models.Shared.Responses.Shared;
using OneOf.Types;

namespace IdentityServer.Api.Repositories
{
    public class UserRepository : IUserRepository
    {
        private IdentityServerDbContext _context;

        public UserRepository(IdentityServerDbContext context)
        {
            _context = context;
        }

        public async Task<CreateResponse<IdResponse>> AddUserAsync(RegisterUserRequest request)
        {

            var existPhone = await _context.Users.Where(x => x.PhoneNumber == request.PhoneNumber.NormalizeString()).CountAsync() > 0;
            var existIdName = await _context.Users.Where(x => x.IdName == request.IdName.NormalizeString()).CountAsync() > 0;

            if (existPhone || existIdName)
            {
                var validationErrorList = new List<ValidationFailed>();
                if (existIdName)
                {
                    validationErrorList.Add(new ValidationFailed()
                    {
                        Field = nameof(request.IdName),
                        Error = $"this {nameof(request.IdName)} is used"
                    });
                }

                if (existPhone)
                {
                    validationErrorList.Add(new ValidationFailed()
                    {
                        Field = nameof(request.PhoneNumber),
                        Error = $"this {nameof(request.PhoneNumber)} is used"
                    });
                }

                return validationErrorList;
            }

            var user = new User
            {
                id = Guid.NewGuid(),
                IdName = request.IdName.NormalizeString(),
                Password = PasswordHash.Hash(request.Password),
                PhoneNumber = request.PhoneNumber.NormalizeString(),
            };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return new Success<IdResponse>(new IdResponse() {  FieldName = "UserId",Id = user.id });
        }

        public async Task<ReadResponse<UserDto>> GetUserAsync(LoginUserRequest request)
        {
            var userDto = await _context.Users.Where(x =>
                    x.PhoneNumber == request.PhoneNumber.Normalize() &&
                    x.Password == PasswordHash.Hash(request.Password))
                .Select(x => new UserDto()
                {
                    id = x.id,
                    PhoneNumber = x.PhoneNumber,
                    IdName = x.IdName,
                    CreatedDate = x.CreatedDate,
                    ModifiedDate = x.ModifiedDate
                }).SingleOrDefaultAsync();

            if (userDto != null)
            {
                return new Success<UserDto>(userDto);
            }

            return new NotFound();
        }


        public async Task<ReadResponse<bool>> ExistPhoneAsync(string phone)
        {
            var exist = await _context.Users.Where(x => x.PhoneNumber == phone.NormalizeString()).CountAsync() > 0;

            return new Success<bool>(exist);
        }

        public async Task<ReadResponse<bool>> ExistIdNameAsync(string idName)
        {
            var exist =  await _context.Users.Where(x => x.IdName == idName.NormalizeString()).CountAsync() > 0;

            return new Success<bool>(exist);
        }
    }
}
