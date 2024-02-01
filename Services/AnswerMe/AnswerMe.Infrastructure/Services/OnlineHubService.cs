using AnswerMe.Application.Common.Interfaces;
using AnswerMe.Application.DTOs.User;
using AnswerMe.Infrastructure.Hubs;
using AnswerMe.Infrastructure.Persistence;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Models.Shared.RepositoriesResponseTypes;
using Models.Shared.Responses.Shared;
using OneOf.Types;

namespace AnswerMe.Infrastructure.Services
{
    public class OnlineHubService : IOnlineHubService
    {
        private readonly ICacheRepository _cacheRepository;
        private IHubContext<OnlineHub> _onlineHubContext;
        private AnswerMeDbContext _context;

        public OnlineHubService(ICacheRepository cacheRepository, IHubContext<OnlineHub> onlineHubContext, AnswerMeDbContext context)
        {
            _cacheRepository = cacheRepository;
            _onlineHubContext = onlineHubContext;
            _context = context;
        }

        public async Task<ReadResponse<BooleanResponse>> IsOnlineAsync(Guid userId)
        {
            var result = await _cacheRepository.GetAsync<UserOnlineDto>("Online-"+userId);

            return new Success<BooleanResponse>(new BooleanResponse()
            { FieldName = "IsOnline", Result = result != null });

        }

        public async Task<CreateResponse<VoidResponse>> NotifyNewPvMessageAsync(Guid roomId, string message)
        {
            var pvList = await _context.PrivateChats
                .Where(chat => chat.id == roomId)
                .ToListAsync();
            var userContactIds = pvList
                .SelectMany(chat => new[] { chat.User1Id, chat.User2Id })
                .ToList();

            
            List<string> onlineContactConnectionIdList= new ();


            foreach (Guid userId in userContactIds)
            {
                var onlineContact = await _cacheRepository.GetAsync<UserOnlineDto>("Online-"+userId);
                if (onlineContact != null)
                {
                    onlineContactConnectionIdList.Add(onlineContact.ConnectionId);
                }
            }
            
            await _onlineHubContext.Clients.Clients(onlineContactConnectionIdList).SendAsync("NotifyNewPvMessage", roomId, message[..Math.Min(10, message.Length)]);

            return new Success<VoidResponse>();
        }

        public async Task<CreateResponse<VoidResponse>> NotifyNewGroupMessageAsync(Guid roomId, string message)
        {
            var userContactIds =await _context.UserGroups
                .Where(gr=>gr.GroupId == roomId)
                .Select(x=>x.UserId)
                .ToListAsync();
        
            
            List<string> onlineContactConnectionIdList= new ();


            foreach (Guid userId in userContactIds)
            {
                var onlineContact = await _cacheRepository.GetAsync<UserOnlineDto>("Online-"+userId);
                if (onlineContact != null)
                {
                    onlineContactConnectionIdList.Add(onlineContact.ConnectionId);
                }
            }
            
            
            await _onlineHubContext.Clients.Clients(onlineContactConnectionIdList).SendAsync("NotifyNewGrMessage", roomId, message[..Math.Min(10, message.Length)]);

            return new Success<VoidResponse>();
        }
        
    }
}
