using IdentityServer.Api.DTOs.User;
using Models.Shared.RepositoriesResponseTypes;
using Models.Shared.Requests.User;
using Models.Shared.Responses.Shared;

namespace IdentityServer.Api.Repositories;

public interface IUserRepository
{
    /// <summary>
    /// Adds a new user asynchronously.
    /// </summary>
    /// <param name="request">The registration request containing user details.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the response with the created user ID.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the registration request is null.</exception>
    Task<CreateResponse<IdResponse>> AddUserAsync(RegisterUserRequest request);
    Task<ReadResponse<UserDto>> GetUserAsync(LoginUserRequest request);

    /// <summary>
    /// Checks if a phone number exists asynchronously.
    /// </summary>
    /// <param name="phone">The phone number to check.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains a ReadResponse&lt;bool&gt; with the existence status of the phone number.</returns>
    Task<ReadResponse<bool>> ExistPhoneAsync(string phone);

    /// <summary>
    /// Checks if a certain idName exists asynchronously.
    /// </summary>
    /// <param name="idName">The idName to check.</param>
    /// <returns>A Task that represents the asynchronous operation. The task result contains a ReadResponse object with a boolean value indicating whether the idName exists or not.</returns>
    Task<ReadResponse<bool>> ExistIdNameAsync(string idName);

}