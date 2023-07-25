using Microsoft.AspNetCore.SignalR;

namespace AnswerMe.Api.Hubs
{
    public sealed class ChatHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            
            var userIdentifier = Context.UserIdentifier;
            Console.WriteLine(userIdentifier);
            await Clients.All.SendAsync("ReceiveMessage", $"{Context.ConnectionId} has joined");
            if (!string.IsNullOrWhiteSpace(userIdentifier))
            {
                await Clients.User(userIdentifier).SendAsync("ReceiveMessage", $"{Context.ConnectionId} AAAAAAA");
            }
            else
            {
                await Clients.All.SendAsync("ReceiveMessage", $"it is null");
            }
            
        }
    }
}
