using AnswerMe.Application.DTOs;
using Models.Shared.RepositoriesResponseTypes;
using Models.Shared.Requests.Shared;
using Models.Shared.Requests.User;
using Models.Shared.Responses.Shared;
using Models.Shared.Responses.User;

namespace AnswerMe.Application.Common.Interfaces;

/// <summary>
/// Represents an interface for interacting with user data in the repository.
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Checks if the specified id is online asynchronously.
    /// </summary>
    /// <param name="id">The id to check if online.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the response indicating whether the id is online or not.
    /// </returns>
    Task<ReadResponse<BooleanResponse>> IsOnlineAsync(Guid id);
    
    /// <summary>
    /// Searches for users based on a keyword asynchronously, with pagination support.
    /// </summary>
    /// <param name="keyWord">The keyword to search for in user profiles.</param>
    /// <param name="request">The pagination request parameters.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains a response with a paged list of user information matching the keyword.
    /// </returns>
    Task<ReadResponse<PagedListResponse<UserResponse>>> SearchUserAsync(string keyWord , PaginationRequest request);

    /// <summary>
    /// Checks if an item with the specified ID exists asynchronously.
    /// </summary>
    /// <param name="id">The ID of the item to check.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a response indicating whether the item exists or not.</returns>
    Task<ReadResponse<BooleanResponse>> ExistAsync(Guid id);

    /// <summary>
    /// AddAsync is a method that adds a user asynchronously in the system.
    /// </summary>
    /// <param name="userDto">The userDto parameter is an instance of the AddUserDto class which contains the information about the user to be added.</param>
    /// <returns>A Task object that represents the asynchronous operation and returns a CreateResponse object with an IdResponse object that contains the ID of the newly created user.</returns>
    Task<CreateResponse<IdResponse>> AddAsync(AddUserDto userDto);

    /// Retrieves a user from the system by their unique identifier asynchronously.
    /// @param id The unique identifier of the user to retrieve.
    /// @return A task that represents the asynchronous operation. The task result contains a
    /// response containing the retrieved user.
    /// /
    Task<ReadResponse<UserResponse>> GetByIdAsync(Guid id);

    /// <summary>
    /// Edits a user asynchronously.
    /// </summary>
    /// <param name="loggedInUserId">The unique identifier of the logged in user.</param>
    /// <param name="request">The request object containing the user data to be edited.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains an UpdateResponse object.</returns>
    Task<UpdateResponse> EditAsync(Guid loggedInUserId, EditUserRequest request);
}