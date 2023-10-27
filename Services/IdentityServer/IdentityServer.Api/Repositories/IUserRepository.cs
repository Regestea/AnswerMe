using IdentityServer.Api.DTOs.User;
using Models.Shared.RepositoriesResponseTypes;
using Models.Shared.Requests.User;
using Models.Shared.Responses.Shared;

namespace IdentityServer.Api.Repositories;

public interface IUserRepository
{
    Task<CreateResponse<IdResponse>> AddUserAsync(RegisterUserRequest request);
    Task<ReadResponse<UserDto>> GetUserAsync(LoginUserRequest request);

    Task<ReadResponse<bool>> ExistPhoneAsync(string phone);
    Task<ReadResponse<bool>> ExistIdNameAsync(string idName);

}