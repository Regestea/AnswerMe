using Models.Shared.Responses.Message;

namespace AnswerMe.Application.Common.Interfaces;

/// <summary>
/// Represents a service for communicating with a group hub.
/// </summary>
public interface IGroupHubService
{
    /// <summary>
    /// Sends a message asynchronously to a specific room.
    /// </summary>
    /// <param name="loggedInUserId">logged in user id</param>
    /// <param name="roomId">The unique identifier of the room.</param>
    /// <param name="messageResponse">The message to be sent.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SendMessageAsync(Guid loggedInUserId,Guid roomId, MessageResponse messageResponse);

    /// <summary>
    /// Updates the specified message in the given room asynchronously.
    /// </summary>
    /// <param name="loggedInUserId">logged in user id</param>
    /// <param name="roomId">The unique identifier of the room</param>
    /// <param name="messageResponse">The new message response data</param>
    /// <returns>A task representing the asynchronous update operation</returns>
    Task UpdateMessageAsync(Guid loggedInUserId,Guid roomId, MessageResponse messageResponse);

    /// <summary>
    /// Removes a message from a room asynchronously.
    /// </summary>
    /// <param name="loggedInUserId">logged in user id</param>
    /// <param name="roomId">The unique identifier of the room.</param>
    /// <param name="messageId">The unique identifier of the message to be removed.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task RemoveMessageAsync(Guid loggedInUserId,Guid roomId, Guid messageId);
}