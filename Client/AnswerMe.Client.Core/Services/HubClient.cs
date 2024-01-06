using System.Net.Http.Headers;
using AnswerMe.Client.Core.DTOs.Base;
using AnswerMe.Client.Core.Services.Interfaces;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;

namespace AnswerMe.Client.Core.Services;

public class HubClient:IHubBase
{
    private readonly HubConnection _connection;
    private readonly AppSettings _appSettings;
    
    public HubClient(ILocalStorageService localStorageService,string hubName, AppSettings appSettings)
    {
        _appSettings = appSettings;
        var connectionString = _appSettings.OnlineHub + hubName;
        _connection = new HubConnectionBuilder()
            .WithUrl(connectionString, option =>
            {
                // option.AccessTokenProvider = 
                //     async () =>
                // {
                //     return await localStorageService.GetItemAsStringAsync("authToken");
                // };
                option.Transports = HttpTransportType.WebSockets;
                option.SkipNegotiation = true;
            }).Build();
    }
    

    public async Task<HubConnectionState> ConnectAsync()
    {
        await _connection.StartAsync();
        _connection.Closed += (sender) =>
        {
            Console.WriteLine("DCDCDC");

            return Task.CompletedTask;
        };
        return _connection.State;
    }

    public async Task DisconnectAsync()
    {
        await _connection.DisposeAsync();
    }
}