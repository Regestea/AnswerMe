using Blazored.LocalStorage;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using Models.Shared.Responses.Message;

namespace AnswerMe.Client.Core.Services;

public class PvHubService
{
    private HubConnection _hubConnection;
    private bool _isConnectionClosedIntentionally = false;
    private string _token = "";
    private string _roomId = "";
    
    public async Task SetToken(ILocalStorageService localStorage)
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
            await Task.Delay(5000);
            if (!_isConnectionClosedIntentionally)
            {
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
    
    public  void ReceivePrivateMessage(Action<MessageResponse> handler)
    {
        _hubConnection.On<MessageResponse>("ReceivePrivateMessage",(messageResponse) =>
        {
            handler.Invoke(messageResponse);
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