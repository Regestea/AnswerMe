using Models.Shared.RepositoriesResponseTypes;
using Models.Shared.Requests.Group;
using Models.Shared.Responses.Group;
using Models.Shared.Responses.Shared;

namespace AnswerMe.Application.Common.Interfaces;

public interface IGroupInviteRepository
{
    Task<ReadResponse<GroupResponse>> GetGroupPreviewAsync(string inviteToken);
    Task<CreateResponse<TokenResponse>> CreateAsync(Guid loggedInUserId, Guid groupId, CreateInviteTokenRequest request);
    Task<CreateResponse<IdResponse>> JoinGroupAsync(Guid loggedInUserId, string inviteToken);
}