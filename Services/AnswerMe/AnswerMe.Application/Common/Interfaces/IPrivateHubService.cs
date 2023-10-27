using Models.Shared.Responses.Message;

namespace AnswerMe.Application.Common.Interfaces;

public interface IPrivateHubService
{
    Task SendMessageAsync(Guid roomId, MessageResponse messageResponse);
    Task UpdateMessageAsync(Guid roomId, MessageResponse messageResponse);
    Task RemoveMessageAsync(Guid roomId, Guid messageId);
}