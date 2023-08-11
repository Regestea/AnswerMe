using IdentityServer.Api.DTOs.User;
using Models.Shared.RepositoriesResponseTypes;
using Models.Shared.Requests.User;
using Models.Shared.Responses.Shared;

namespace IdentityServer.Api.Repositories;

public interface IUserRepository
{
    Task<CreateResponse<IdResponse>> AddUser(RegisterUserRequest request);
    Task<ReadResponse<UserDto>> GetUser(LoginUserRequest request);

    Task<ReadResponse<bool>> ExistPhone(string phone);
    Task<ReadResponse<bool>> ExistIdName(string idName);

}