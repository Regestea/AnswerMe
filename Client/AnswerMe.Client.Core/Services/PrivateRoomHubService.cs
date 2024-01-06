using AnswerMe.Client.Core.DTOs.Base;
using AnswerMe.Client.Core.Services.Interfaces;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.SignalR.Client;
using Models.Shared.Responses.Message;

namespace AnswerMe.Client.Core.Services;

public class PrivateRoomHubService:IPrivateRoomHubService
{
    // private HubClient _hubClient;
    // private AppSettings _appSettings;
    // private ILocalStorageService _localStorageService;
    // public PrivateRoomHubService(HubClient hubClient, AppSettings appSettings, ILocalStorageService localStorageService)
    // {
    //     _appSettings = appSettings;
    //     _localStorageService = localStorageService;
    //     _hubClient = new HubClient(_localStorageService,appSettings.PrivateRoomHub ?? throw new InvalidOperationException());
    // }


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