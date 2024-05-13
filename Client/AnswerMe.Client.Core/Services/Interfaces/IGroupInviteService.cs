using Models.Shared.RepositoriesResponseTypes;
using Models.Shared.Requests.Group;
using Models.Shared.Requests.Shared;
using Models.Shared.Responses.Group;
using Models.Shared.Responses.Shared;

namespace AnswerMe.Client.Core.Services.Interfaces;

public interface IGroupInviteService
{
    Task<ReadResponse<PreviewGroupResponse>> GetGroupInvitePreviewAsync(TokenRequest request);
    Task<CreateResponse<TokenResponse>> CreateAsync(Guid groupId, CreateInviteTokenRequest request);
    Task<CreateResponse<IdResponse>> JoinGroupAsync(string inviteToken);
}