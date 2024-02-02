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
    private readonly HubConnection hubConnection;
    private string _token = "";

    public OnlineHubService(IJSRuntime jsRuntime)
    {
        hubConnection = new HubConnectionBuilder()
            .WithUrl("https://localhost:7156/Online-User", option =>
            {
                option.AccessTokenProvider = () =>
                    Task.FromResult(_token)!;
            })
            .Build();

        hubConnection.Closed += ConnectToHub().Wait();
    }


    public async Task SetToken(ILocalStorageService localStorage)
    {
        _token = await localStorage.GetItemAsStringAsync("authToken");
    }

    

    public async Task ConnectToHub()
    {
        try
        {
            await hubConnection.StartAsync();
    
            await JsRuntime.ReplaceClass(NavMenu.ElementIds.HeaderAvatar.ToString(),"offline","online");
    
            await JsRuntime.SetInnerText(NavMenu.ElementIds.HeaderText.ToString(), "Answer Me");
        }
        catch (Exception e)
        {
            await OnReconnect(() =>  _ = ConnectToHub());
        }
    }
    
    public async Task OnReconnect(Action handler)
    {
        await JsRuntime.ReplaceClass(NavMenu.ElementIds.HeaderAvatar.ToString(),"online","offline");
    
        await JsRuntime.SetInnerText(NavMenu.ElementIds.HeaderText.ToString(), "Connecting .....");
        handler.Invoke();
        await Task.Delay(5000);
    }

    private async Task OnClosed(Action handler)
    {
        // Log the disconnection event
        await JsRuntime.ReplaceClass(NavMenu.ElementIds.HeaderAvatar.ToString(),"online","offline");
    
        await JsRuntime.SetInnerText(NavMenu.ElementIds.HeaderText.ToString(), "Connecting .....");
        
        handler.Invoke();

        await ConnectToHub();
        await Task.Delay(5000);
    }


    public Task RegisterUserWentOnline(Action<Guid> handler)
    {
        hubConnection.On<Guid>("UserWentOnline", async (userId) =>
        {
            Console.WriteLine("user went online " + userId);
            handler.Invoke(userId);
            await JsRuntime.ReplaceClass(userId + "-Status", "offline", "online");
        });
    }

    public Task RegisterUserWentOffline(Action<Guid> handler)
    {
        hubConnection.On<Guid>("UserWentOffline", async (userId) =>
        {
            Console.WriteLine("user went offline " + userId);

            handler.Invoke(userId);
            await JsRuntime.ReplaceClass(userId + "-Status", "online", "offline");
        });
    }

    public Task RegisterNotifyNewPvMessage(Action<Guid, string> handler)
    {
        hubConnection.On<Guid, string>("NotifyNewPvMessage", async (roomId, message) =>
        {
            Console.WriteLine("NotifyNewPvMessage" + roomId + message);
            handler.Invoke(roomId, message);
            NavMenu.SetPvLastMessage(roomId, message);
        });
    }

    public Task RegisterNotifyNewGroupMessage(Action<Guid, string> handler)
    {
        hubConnection.On<Guid, string>("NotifyNewGroupMessage", async (roomId, message) =>
        {
            Console.WriteLine("NotifyNewPvMessage" + roomId + message);
            handler.Invoke(roomId, message);
            NavMenu.SetGrLastMessage(roomId, message);
        });
    }
}