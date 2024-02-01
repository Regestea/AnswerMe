using Models.Shared.RepositoriesResponseTypes;
using Models.Shared.Requests.Message;
using Models.Shared.Requests.Shared;
using Models.Shared.Responses.Message;
using Models.Shared.Responses.Shared;

namespace AnswerMe.Application.Common.Interfaces;

/// Represents a service for managing private messages.
/// /
public interface IPrivateMessageService
{
    /// <summary>
    /// Sends a message request asynchronously.
    /// </summary>
    /// <param name="loggedInUserId">The ID of the logged in user.</param>
    /// <param name="roomId">The ID of the room.</param>
    /// <param name="request">The message request to be sent.</param>
    /// <returns>The task representing the asynchronous operation. The task result contains the response with the created message ID.</returns>
    Task<CreateResponse<IdResponse>> SendAsync(Guid loggedInUserId, Guid roomId, SendMessageRequest request);

    /// <summary>
    /// Retrieves a list of messages in a specified room
    /// based on the provided pagination request.
    /// </summary>
    /// <param name="loggedInUserId">The unique identifier of the logged-in user</param>
    /// <param name="roomId">The unique identifier of the room to retrieve messages from</param>
    /// <param name="jumpToUnRead">should jump to unRead page</param>
    /// <param name="request">The pagination request specifying
    /// the number of items to return and the page number</param>
    /// <returns>A task that represents the asynchronous
    /// operation. The task result contains a response
    /// object containing a paged list of message responses</returns>
    Task<ReadResponse<PagedListResponse<MessageResponse>>> GetListAsync(Guid loggedInUserId, Guid roomId,bool jumpToUnRead, PaginationRequest request);
    
    /// <summary>
    /// Get details of a specific message.
    /// </summary>
    /// <param name="loggedInUserId">The ID of the logged-in user making the request.</param>
    /// <param name="messageId">The ID of the message to retrieve.</param>
    /// <returns>
    /// Returns the details of the specified message if successful.
    /// Returns NotFound if the message with the given ID is not found.
    /// </returns>
    Task<ReadResponse<MessageResponse>> GetAsync(Guid loggedInUserId, Guid messageId);

    /// <summary>
    /// Updates a message asynchronously.
    /// </summary>
    /// <param name="loggedInUserId">The unique identifier of the logged-in user.</param>
    /// <param name="messageId">The unique identifier of the message to be updated.</param>
    /// <param name="request">The request object containing the updated message details.</param>
    /// <returns>A task representing the async operation with the response of the update.</returns>
    Task<UpdateResponse> UpdateAsync(Guid loggedInUserId, Guid messageId, EditMessageRequest request);

    /// <summary>
    /// Updates the specified media of a message asynchronously
    /// .
    /// </summary>
    /// <param name="loggedInUserId">The ID of the logged in user.</param>
    /// <param name="messageId">The ID of the message.</param>
    /// <param name="mediaId">The ID of the media to be updated.</param>
    /// <param name="request">The request object containing the updated media information.</param>
    /// <returns>A Task representing the async operation. The
    /// task result contains an UpdateResponse object.</returns>
    Task<UpdateResponse> UpdateMediaAsync(Guid loggedInUserId, Guid messageId, Guid mediaId, EditMessageMediaRequest request);

    /// <summary>
    /// Deletes a message asynchronously.
    /// </summary>
    /// <param name="loggedInUserId">The ID of the user who is logged in.</param>
    /// <param name="messageId">The ID of the message to be deleted.</param>
    /// <returns>A Task representing the asynchronous operation. The resulting DeleteResponse object contains information about the deletion status.</returns>
    Task<DeleteResponse> DeleteAsync(Guid loggedInUserId, Guid messageId);

    /// <summary>
    /// Deletes a media item associated with a message asynchronously.
    /// </summary>
    /// <param name="loggedInUserId">The unique identifier of the logged-in user.</param>
    /// <param name="messageId">The unique identifier of the message.</param>
    /// <param name="mediaId">The unique identifier of the media item to be deleted.</param>
    /// <returns>A task representing the asynchronous operation. The result of the task contains a DeleteResponse indicating the status of the delete operation.</returns>
    Task<DeleteResponse> DeleteMediaAsync(Guid loggedInUserId, Guid messageId, Guid mediaId);

}
