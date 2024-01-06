using AnswerMe.Client.Core.Services.Interfaces;
using Microsoft.AspNetCore.SignalR.Client;
using Models.Shared.Responses.Message;

namespace AnswerMe.Client.Core.Services;

public class GroupRoomHubService:IGroupRoomHubService
{
    public Task<HubConnectionState> ConnectAsync()
    {
        throw new NotImplementedException();
    }

    public Task DisconnectAsync()
    {
        throw new NotImplementedException();
    }

    public Task ReceiveMessageAsync(Guid roomId, MessageResponse messageResponse)
    {
        throw new NotImplementedException();
    }

    public Task EditMessageAsync(Guid roomId, MessageResponse messageResponse)
    {
        throw new NotImplementedException();
    }

    public Task RemoveMessageAsync(Guid roomId, Guid messageId)
    {
        throw new NotImplementedException();
    }
}