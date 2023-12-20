using Models.Shared.RepositoriesResponseTypes;
using Models.Shared.Requests.Group;
using Models.Shared.Requests.Shared;
using Models.Shared.Responses.Group;
using Models.Shared.Responses.Shared;

namespace AnswerMe.Client.Core.Services.Interfaces;

public interface IGroupService
{
    Task<ReadResponse<GroupResponse>> GetByIdAsync(Guid groupId);
    Task<ReadResponse<PagedListResponse<GroupResponse>>> GetGroupsAsync(PaginationRequest paginationRequest);
    Task<ReadResponse<PagedListResponse<PreviewGroupUserResponse>>> UserListAsync(Guid groupId, PaginationRequest paginationRequest);
    Task<CreateResponse<IdResponse>> SetUserAsAdminAsync(Guid groupId, Guid userId);
    Task<DeleteResponse> RemoveUserFromAdminsAsync(Guid groupId, Guid userId);
    Task<CreateResponse<IdResponse>> CreateAsync(CreateGroupRequest request);
    Task<UpdateResponse> EditAsync(Guid groupId, EditGroupRequest request);
    Task<CreateResponse<IdResponse>> JoinUserAsync(Guid groupId, Guid joinUserId);
    Task<DeleteResponse> KickUserAsync(Guid groupId, Guid kickUserId);
    Task<DeleteResponse> LeaveGroupAsync(Guid groupId);
}