using Models.Shared.RepositoriesResponseTypes;
using Models.Shared.Requests.Shared;
using Models.Shared.Responses.PrivateRoom;
using Models.Shared.Responses.Shared;

namespace AnswerMe.Application.Common.Interfaces;

public interface IPrivateRoomRepository
{
    Task<ReadResponse<PrivateRoomResponse>> GetAsync(Guid loggedInUserId, Guid roomId);
    Task<ReadResponse<PagedListResponse<PrivateRoomResponse>>> GetListAsync(Guid loggedInUserId, PaginationRequest paginationRequest);
    Task<ReadResponse<RoomLastSeenResponse>> GetLastSeenAsync(Guid loggedInUserId, Guid roomId, Guid userId);
}

