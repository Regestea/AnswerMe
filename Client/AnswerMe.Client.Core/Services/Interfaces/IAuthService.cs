using Models.Shared.RepositoriesResponseTypes;
using Models.Shared.Requests.User;
using Models.Shared.Responses.Shared;

namespace AnswerMe.Client.Core.Services.Interfaces
{
    public interface IAuthService
    {
        // Register return id
        Task<CreateResponse<IdResponse>> RegisterAsync(RegisterUserRequest request);

        // Login return string token
        Task<ReadResponse<TokenResponse>> LoginAsync(LoginUserRequest request);
    }
}