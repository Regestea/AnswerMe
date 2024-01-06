using Models.Shared.RepositoriesResponseTypes;
using Models.Shared.Responses.Shared;

namespace AnswerMe.Client.Core.Services.Interfaces;

public interface IOnlineHubService:IHubBase
{
    Task<ReadResponse<IdResponse>> WentOnline();
}