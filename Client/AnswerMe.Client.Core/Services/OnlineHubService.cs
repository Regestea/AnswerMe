using AnswerMe.Client.Core.DTOs.Base;
using AnswerMe.Client.Core.Extensions;
using AnswerMe.Client.Core.Services.Interfaces;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Http.Connections.Client;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using Models.Shared.RepositoriesResponseTypes;
using Models.Shared.Responses.Shared;

namespace AnswerMe.Client.Core.Services;

public class OnlineHubService
{
    private readonly HubConnection _hubConnection;
    private string _token = "";

    public OnlineHubService()
    {
        _hubConnection = new HubConnectionBuilder()
            .WithUrl("https://localhost:7156/Online-User", option =>
            {
                option.AccessTokenProvider = () =>
                    Task.FromResult(_token)!;
            })
            .Build();
    }


    public async Task SetTokenAsync(ILocalStorageService localStorage)
    {
        _token = await localStorage.GetItemAsStringAsync("authToken");
    }

    

    public async Task ConnectToHubAsync(Action handler)
    {

        _hubConnection.Closed += async _ =>
        {
            await Task.Delay(5000);
            await ConnectToHubAsync(handler.Invoke);
        };
        
        try
        {
            await _hubConnection.StartAsync();
            handler.Invoke();
        }
        catch (Exception e)
        {
            await Task.Delay(5000);
            await ConnectToHubAsync(handler.Invoke);
        }
    }

    public void OnClosed(Action handler)
    {
        _hubConnection.Closed += _ =>
        {
            handler.Invoke();
            return Task.CompletedTask;
        };
    }


    public  void RegisterUserWentOnline(Action<Guid> handler)
    {
        _hubConnection.On<Guid>("UserWentOnline",  (userId) =>
        {
            Console.WriteLine("user went online " + userId);
            handler.Invoke(userId);
        });
    }

    public void RegisterUserWentOffline(Action<Guid> handler)
    {
        _hubConnection.On<Guid>("UserWentOffline",  (userId) =>
        {
            Console.WriteLine("user went offline " + userId);
            handler.Invoke(userId);
        });
    }

    public void RegisterNotifyNewPvMessage(Action<Guid, string> handler)
    {
        _hubConnection.On<Guid, string>("NotifyNewPvMessage", async (roomId, message) =>
        {
            Console.WriteLine("NotifyNewPvMessage" + roomId + message);
            handler.Invoke(roomId, message);
        });
    }

    public void RegisterNotifyNewGroupMessage(Action<Guid, string> handler)
    {
        _hubConnection.On<Guid, string>("NotifyNewGroupMessage", async (roomId, message) =>
        {
            Console.WriteLine("NotifyNewPvMessage" + roomId + message);
            handler.Invoke(roomId, message);
        });
    }
}