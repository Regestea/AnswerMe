using Models.Shared.RepositoriesResponseTypes;
using Models.Shared.Requests.Group;
using Models.Shared.Requests.Shared;
using Models.Shared.Responses.Group;
using Models.Shared.Responses.Shared;

namespace AnswerMe.Application.Common.Interfaces;

public interface IGroupRepository
{
    Task<ReadResponse<GroupResponse>> GetAsync(Guid loggedInUserId, Guid groupId);
    Task<ReadResponse<PagedListResponse<GroupResponse>>> GetListAsync(Guid loggedInUserId, PaginationRequest paginationRequest);
    Task<ReadResponse<PagedListResponse<PreviewGroupUserResponse>>> UserListAsync(Guid loggedInUserId, Guid groupId, PaginationRequest paginationRequest);
    Task<CreateResponse<IdResponse>> SetUserAsAdminAsync(Guid loggedInUserId, Guid groupId, Guid userId);
    Task<DeleteResponse> RemoveUserFromAdminsAsync(Guid loggedInUserId, Guid groupId, Guid userId);
    Task<CreateResponse<IdResponse>> CreateAsync(Guid loggedInUserId, CreateGroupRequest request);
    Task<UpdateResponse> EditAsync(Guid loggedInUserId, Guid groupId, EditGroupRequest request);
    Task<CreateResponse<IdResponse>> JoinUserAsync(Guid loggedInUserId, Guid groupId, Guid joinUserId);
    Task<DeleteResponse> KickUserAsync(Guid loggedInUserId, Guid groupId, Guid kickUserId);
    Task<DeleteResponse> LeaveGroupAsync(Guid loggedInUserId, Guid groupId);
}


