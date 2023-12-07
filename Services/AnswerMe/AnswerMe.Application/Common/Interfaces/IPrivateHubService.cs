using Models.Shared.Responses.Message;

namespace AnswerMe.Application.Common.Interfaces;

/// <summary>
/// Represents a service for managing private chat messages in a hub.
/// </summary>
public interface IPrivateHubService
{
    /// <summary>
    /// Sends a message asynchronously to a specific room.
    /// </summary>
    /// <param name="roomId">The unique identifier of the room.</param>
    /// <param name="messageResponse">The message to be sent, along with its properties.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SendMessageAsync(Guid roomId, MessageResponse messageResponse);

    /// <summary>
    /// Updates a message in a specified room asynchronously.
    /// </summary>
    /// <param name="roomId">The unique identifier of the room.</param>
    /// <param name="messageResponse">The updated message response object.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateMessageAsync(Guid roomId, MessageResponse messageResponse);

    /// <summary>
    /// Removes a message from a specific room asynchronously.
    /// </summary>
    /// <param name="roomId">The unique identifier of the room.</param>
    /// <param name="messageId">The unique identifier of the message.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task RemoveMessageAsync(Guid roomId, Guid messageId);
}