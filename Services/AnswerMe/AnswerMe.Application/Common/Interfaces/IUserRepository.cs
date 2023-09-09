using AnswerMe.Application.DTOs;
using Models.Shared.RepositoriesResponseTypes;
using Models.Shared.Requests.Shared;
using Models.Shared.Requests.User;
using Models.Shared.Responses.Shared;
using Models.Shared.Responses.User;

namespace AnswerMe.Application.Common.Interfaces;

public interface IUserRepository
{
    Task<ReadResponse<bool>> ExistUserAsync(Guid id);
    Task<CreateResponse<IdResponse>> AddUserAsync(AddUserDto userDto);
    Task<ReadResponse<UserResponse>> GetUserByIdAsync(Guid id);
    Task<UpdateResponse> EditUserAsync(Guid id, EditUserRequest request);
}