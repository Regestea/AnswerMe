using AnswerMe.Application.DTOs.User;
using AnswerMe.Application.RepositoriesResponseTypes;
using Models.Shared.Requests.User;
using Models.Shared.Responses.Shared;

namespace AnswerMe.Application.Common.Interfaces;

public interface IUserRepository
{
    Task<CreateResponse<IdResponse>> AddUser(RegisterUserRequest request);
    Task<ReadResponse<UserDto>> GetUser(LoginUserRequest request);
    Task<UpdateResponse> EditProfileImage(EditProfileImageRequest request);

    Task<ReadResponse<bool>> ExistPhone(string phone);
    Task<ReadResponse<bool>> ExistIdName(string idName);

}