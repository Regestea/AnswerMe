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
    }
}
