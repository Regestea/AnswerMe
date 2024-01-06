using Models.Shared.RepositoriesResponseTypes;
using Models.Shared.Requests.Shared;
using Models.Shared.Responses.PrivateRoom;
using Models.Shared.Responses.Shared;

namespace AnswerMe.Application.Common.Interfaces;

/// <summary>
/// Represents a repository for accessing private room data.
/// </summary>
public interface IPrivateRoomRepository
{
    /// <summary>
    /// Retrieves the private room information asynchronously.
    /// </summary>
    /// <param name="loggedInUserId">The ID of the user who is logged in.</param>
    /// <param name="roomId">The ID of the private room to retrieve.</param>
    /// <returns>A task representing the asynchronous operation. The task's result contains the response with the private room information.</returns>
    Task<ReadResponse<PrivateRoomResponse>> GetAsync(Guid loggedInUserId, Guid roomId);

    /// <summary>
    /// Creates a relationship between two users asynchronously.
    /// </summary>
    /// <param name="loggedInUserId">The ID of the user initiating the relationship.</param>
    /// <param name="contactId">The ID of the user to establish a relationship with.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains a response with the ID of the created relationship.
    /// </returns>
    Task<CreateResponse<IdResponse>> CreateAsync(Guid loggedInUserId, Guid contactId);

    /// <summary>
    /// Retrieves a paged list of private rooms asynchronously
    /// .
    /// </summary>
    /// <param name="loggedInUserId">The ID of the logged in user.</param>
    /// <param name="paginationRequest">The pagination request object.</param>
    /// <returns>A task that represents the asynchronous
    /// operation. The task result contains a response
    /// object
    /// containing the paged list of private rooms.</returns>
    Task<ReadResponse<PagedListResponse<PrivateRoomResponse>>> GetListAsync(Guid loggedInUserId,
        PaginationRequest paginationRequest);

    /// <summary>
    /// Retrieves the last seen information for a specific user in a room.
    /// </summary>
    /// <param name="loggedInUserId">The unique identifier of the logged-in user.</param>
    /// <param name="roomId">The unique identifier of the room.</param>
    /// <param name="userId">The unique identifier of the user for whom to retrieve the last seen information.</param>
    /// <returns>A task that represents the asynchronous operation with the result
    /// being a <see cref="ReadResponse{T}"/> containing the last seen information of type <see cref="RoomLastSeenResponse"/>.</returns>
    Task<ReadResponse<RoomLastSeenResponse>> GetLastSeenAsync(Guid loggedInUserId, Guid roomId, Guid userId);
}