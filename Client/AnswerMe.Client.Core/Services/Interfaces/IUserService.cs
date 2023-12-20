using Models.Shared.RepositoriesResponseTypes;
using Models.Shared.Requests.User;
using Models.Shared.Responses.Shared;
using Models.Shared.Responses.User;

namespace AnswerMe.Client.Core.Services.Interfaces;

public interface IUserService
{
    Task<ReadResponse<UserResponse>> GetUserAsync();
    Task<UpdateResponse> EditUserAsync(EditUserRequest request);
    Task<ReadResponse<UserResponse>> GetUserByIdAsync(Guid id);
    Task<ReadResponse<BooleanResponse>> IsOnlineAsync(Guid id);
    Task<ReadResponse<BooleanResponse>> ExistsAsync(Guid id);
}