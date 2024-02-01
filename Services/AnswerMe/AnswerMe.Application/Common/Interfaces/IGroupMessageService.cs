using Models.Shared.RepositoriesResponseTypes;
using Models.Shared.Requests.Message;
using Models.Shared.Requests.Shared;
using Models.Shared.Responses.Message;
using Models.Shared.Responses.Shared;

namespace AnswerMe.Application.Common.Interfaces;

/// <summary>
/// Represents a service for managing group messages.
/// </summary>
public interface IGroupMessageService
{
    /// <summary>
    /// Sends a message asynchronously.
    /// </summary>
    /// <param name="loggedInUserId">The ID of the logged-in user.</param>
    /// <param name="groupId">The ID of the group to send the message to.</param>
    /// <param name="request">The request containing the message details.</param>
    /// <returns>A task representing the asynchronous operation. The task result
    /// is of type CreateResponse&lt;IdResponse&gt;, which contains the response data.</returns>
    Task<CreateResponse<IdResponse>> SendAsync(Guid loggedInUserId, Guid groupId, SendMessageRequest request);

    /// <summary>
    /// Retrieves a list of messages from a specified
    /// group with pagination.
    /// </summary>
    /// <param name="loggedInUserId">The unique identifier of the logged-in user.</param>
    /// <param name="groupId">The unique identifier of the group.</param>
    /// <param name="request">The pagination request parameters.</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// .
    /// The task result contains the response of the
    /// read operation, including the list of messages
    /// .
    /// The response also includes pagination details
    /// for the returned list.
    /// </returns>
    Task<ReadResponse<PagedListResponse<MessageResponse>>> GetListAsync(Guid loggedInUserId, Guid groupId, PaginationRequest request);
    
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
    /// <param name="loggedInUserId">The ID of the logged-in user performing the update.</param>
    /// <param name="messageId">The ID of the message to be updated.</param>
    /// <param name="request">The request containing the updated message details.</param>
    /// <returns>A task that represents the asynchronous update operation. The task result contains an UpdateResponse object.</returns>
    Task<UpdateResponse> UpdateAsync(Guid loggedInUserId, Guid messageId, EditMessageRequest request);

    /// <summary>
    /// Updates the media of a specific message asynchronously
    /// .
    /// </summary>
    /// <param name="loggedInUserId">The unique identifier of the logged-in user.</param>
    /// <param name="messageId">The unique identifier of the message containing the media.</param>
    /// <param name="mediaId">The unique identifier of the media to be updated.</param>
    /// <param name="request">The request object containing the new media data.</param>
    /// <returns>A Task that represents the asynchronous operation
    /// . The task result contains the response of the update operation.</returns>
    Task<UpdateResponse> UpdateMediaAsync(Guid loggedInUserId, Guid messageId, Guid mediaId, EditMessageMediaRequest request);

    /// <summary>
    /// Deletes a message asynchronously.
    /// </summary>
    /// <param name="loggedInUserId">The ID of the logged in user.</param>
    /// <param name="messageId">The ID of the message to delete.</param>
    /// <returns>A task representing the asynchronous operation with a DeleteResponse containing the deletion result.</returns>
    Task<DeleteResponse> DeleteAsync(Guid loggedInUserId, Guid messageId);

    /// <summary>
    /// Deletes a media file associated with a given message by its ID.
    /// </summary>
    /// <param name="loggedInUserId">The ID of the logged-in user.</param>
    /// <param name="messageId">The ID of the message.</param>
    /// <param name="mediaId">The ID of the media file to be deleted.</param>
    /// <returns>A task that represents the asynchronous delete operation. The task result contains the delete response.</returns>
    Task<DeleteResponse> DeleteMediaAsync(Guid loggedInUserId, Guid messageId, Guid mediaId);
}