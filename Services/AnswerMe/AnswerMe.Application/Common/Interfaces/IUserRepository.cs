using AnswerMe.Application.DTOs;
using Models.Shared.RepositoriesResponseTypes;
using Models.Shared.Requests.Shared;
using Models.Shared.Requests.User;
using Models.Shared.Responses.Shared;
using Models.Shared.Responses.User;

namespace AnswerMe.Application.Common.Interfaces;

public interface IUserRepository
{
    Task<ReadResponse<BooleanResponse>> IsOnlineAsync(Guid id);
    Task<ReadResponse<BooleanResponse>> ExistAsync(Guid id);
    Task<CreateResponse<IdResponse>> AddAsync(AddUserDto userDto);
    Task<ReadResponse<UserResponse>> GetByIdAsync(Guid id);
    Task<UpdateResponse> EditAsync(Guid loggedInUserId, EditUserRequest request);
}