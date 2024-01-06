using Microsoft.AspNetCore.SignalR.Client;

namespace AnswerMe.Client.Core.Services.Interfaces;

public interface IHubBase
{
    Task<HubConnectionState> ConnectAsync();
    Task DisconnectAsync();
}