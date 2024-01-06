using AnswerMe.Client.Core.DTOs.Base;
using AnswerMe.Client.Core.Services.Interfaces;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.SignalR.Client;
using Models.Shared.RepositoriesResponseTypes;
using Models.Shared.Responses.Shared;

namespace AnswerMe.Client.Core.Services;

public class OnlineHubService:IOnlineHubService
{
    private HubClient _hubClient;
    private ILocalStorageService _storageService;
    private AppSettings _appSettings;

    public OnlineHubService( ILocalStorageService storageService, AppSettings appSettings)
    {
        _storageService = storageService;
        _appSettings = appSettings;
         _hubClient = new HubClient(_storageService, "Online-User", _appSettings);
    }

    public async Task<HubConnectionState> ConnectAsync()
    {
      return await _hubClient.ConnectAsync();
    }

    public Task DisconnectAsync()
    {
        throw new NotImplementedException();
    }

    public Task<ReadResponse<IdResponse>> WentOnline()
    {
        throw new NotImplementedException();
    }
}