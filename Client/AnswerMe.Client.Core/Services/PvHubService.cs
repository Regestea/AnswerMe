using Blazored.LocalStorage;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using Models.Shared.Responses.Message;
using Models.Shared.Responses.Shared;

namespace AnswerMe.Client.Core.Services;

public class PvHubService
{
    private HubConnection _hubConnection;
    private bool _isConnectionClosedIntentionally = false;
    private string _token = "";
    private string _roomId = "";
    
    public async Task SetTokenAsync(ILocalStorageService localStorage)
    {
        _token = await localStorage.GetItemAsStringAsync("authToken");
    }

    public void SetRoomId(Guid roomId)
    {
        _isConnectionClosedIntentionally = false;
        _roomId=roomId.ToString();
         _hubConnection = new HubConnectionBuilder()
             .WithUrl($"https://localhost:7156/Private-Chat/?RoomId={_roomId}", option =>
             {
                 option.AccessTokenProvider = () =>
                     Task.FromResult(_token)!;
             }).Build();
    }

    

    public async Task ConnectToHubAsync(Action handler)
    {
        _hubConnection.Closed += async _ =>
        {
            if (!_isConnectionClosedIntentionally)
            {
                await Task.Delay(5000);
                await ConnectToHubAsync(handler.Invoke);
            }
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

    public async Task DisconnectAsync()
    {
        _isConnectionClosedIntentionally = true;
        await _hubConnection.StopAsync();
    }
    
    public void OnClosed(Action handler)
    {
        _hubConnection.Closed += _ =>
        {
            handler.Invoke();
            
            return Task.CompletedTask;
        };
    }
    
    public  void NewPvMessage(Action<MessageResponse> handler)
    {
        _hubConnection.On<MessageResponse>("NewPVMessage",(messageResponse) =>
        {
            handler.Invoke(messageResponse);
        });
    }
    
    public  void EditPvMessage(Action<MessageResponse> handler)
    {
        _hubConnection.On<MessageResponse>("EditPVMessage",(messageResponse) =>
        {
            handler.Invoke(messageResponse);
        });
    }
    
    public  void RemovePvMessage(Action<IdResponse> handler)
    {
        _hubConnection.On<IdResponse>("RemovePVMessage",(messageIdResponse) =>
        {
            handler.Invoke(messageIdResponse);
        });
    }
    
    

    public  void JoinedRoom(Action<Guid> handler)
    {
        _hubConnection.On<Guid>("JoinedRoom", (userId) =>
        { 
            handler.Invoke(userId);
        });
    }

    public  void LeftRoom(Action<Guid> handler)
    {
        _hubConnection.On<Guid>("LeftRoom",(userId) =>
        {
            handler.Invoke(userId);
        });
    }

}