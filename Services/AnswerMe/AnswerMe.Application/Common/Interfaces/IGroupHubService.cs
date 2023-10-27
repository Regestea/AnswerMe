using Models.Shared.Responses.Message;

namespace AnswerMe.Application.Common.Interfaces;

public interface IGroupHubService
{
    Task SendMessageAsync(Guid roomId, MessageResponse messageResponse);
    Task UpdateMessageAsync(Guid roomId, MessageResponse messageResponse);
    Task RemoveMessageAsync(Guid roomId, Guid messageId);
}