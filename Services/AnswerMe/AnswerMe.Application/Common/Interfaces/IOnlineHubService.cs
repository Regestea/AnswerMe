using Models.Shared.RepositoriesResponseTypes;
using Models.Shared.Responses.Message;
using Models.Shared.Responses.Shared;

namespace AnswerMe.Application.Common.Interfaces;

public interface IOnlineHubService
{
    Task<ReadResponse<BooleanResponse>> IsOnlineAsync(Guid userId);
}