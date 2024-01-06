using Models.Shared.Responses.Message;

namespace AnswerMe.Client.Core.Services.Interfaces;

public interface IPrivateRoomHubService:IHubBase
{
    Task ReceiveMessageAsync(Guid roomId, MessageResponse messageResponse);
    
    Task EditMessageAsync(Guid roomId, MessageResponse messageResponse);

    Task RemoveMessageAsync(Guid roomId, Guid messageId);
}