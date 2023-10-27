using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnswerMe.Application.Common.Interfaces;
using AnswerMe.Application.DTOs.User;
using AnswerMe.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;
using Models.Shared.RepositoriesResponseTypes;
using Models.Shared.Responses.Shared;
using OneOf.Types;

namespace AnswerMe.Infrastructure.Repositories
{
    public class OnlineHubService : IOnlineHubService
    {
        private readonly ICacheRepository _cacheRepository;

        public OnlineHubService(ICacheRepository cacheRepository)
        {
            _cacheRepository = cacheRepository;
        }

        public async Task<ReadResponse<BooleanResponse>> IsOnlineAsync(Guid userId)
        {
            var result = await _cacheRepository.GetAsync<UserOnlineDto>(userId.ToString());

            return new Success<BooleanResponse>(new BooleanResponse()
            { FieldName = "IsOnline", Result = result != null });

        }
    }
}
