using AnswerMe.Client.Core.DTOs.Base;
using AnswerMe.Client.Core.Extensions;
using AnswerMe.Client.Core.Services.Interfaces;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Http.Connections.Client;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using Models.Shared.RepositoriesResponseTypes;
using Models.Shared.Responses.Message;
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


    public  void UserWentOnline(Action<Guid> handler)
    {
        _hubConnection.On<Guid>("UserWentOnline",  (userId) =>
        {
            handler.Invoke(userId);
        });
    }

    public void UserWentOffline(Action<Guid> handler)
    {
        _hubConnection.On<Guid>("UserWentOffline",  (userId) =>
        {
            handler.Invoke(userId);
        });
    }

    public void NotifyNewPvMessage(Action<RoomNotifyResponse> handler)
    {
        _hubConnection.On<RoomNotifyResponse>("NotifyNewPVMessage", async (roomNotifyResponse) =>
        {
            handler.Invoke(roomNotifyResponse);
        });
    }
    
    public void NotifyEditPvMessage(Action<RoomNotifyResponse> handler)
    {
        _hubConnection.On<RoomNotifyResponse>("NotifyEditPVMessage", async (roomNotifyResponse) =>
        {
            handler.Invoke(roomNotifyResponse);
        });
    }
    
    public void NotifyRemovePvMessage(Action<RoomNotifyResponse> handler)
    {
        _hubConnection.On<RoomNotifyResponse>("NotifyRemovePVMessage", async (roomNotifyResponse) =>
        {
            handler.Invoke(roomNotifyResponse);
        });
    }
    
    
    
    

    public void NotifyNewGrMessage(Action<RoomNotifyResponse> handler)
    {
        _hubConnection.On<RoomNotifyResponse>("NotifyNewGRMessage", async (roomNotifyResponse) =>
        {
            handler.Invoke(roomNotifyResponse);
        });
    }
    
    public void NotifyEditGrMessage(Action<RoomNotifyResponse> handler)
    {
        _hubConnection.On<RoomNotifyResponse>("NotifyEditGRMessage", async (roomNotifyResponse) =>
        {
            handler.Invoke(roomNotifyResponse);
        });
    }
    
    public void NotifyRemoveGrMessage(Action<RoomNotifyResponse> handler)
    {
        _hubConnection.On<RoomNotifyResponse>("NotifyRemoveGRMessage", async (roomNotifyResponse) =>
        {
            handler.Invoke(roomNotifyResponse);
        });
    }
}