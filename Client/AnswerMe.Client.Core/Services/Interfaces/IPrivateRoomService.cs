using Models.Shared.RepositoriesResponseTypes;
using Models.Shared.Requests.Shared;
using Models.Shared.Responses.PrivateRoom;
using Models.Shared.Responses.Shared;

namespace AnswerMe.Client.Core.Services.Interfaces;

public interface IPrivateRoomService
{
    Task<ReadResponse<PagedListResponse<PrivateRoomResponse>>> GetPrivateRoomsAsync(PaginationRequest request);
    Task<ReadResponse<BooleanResponse>> GetUserIsOnlineInRoom(Guid roomId, Guid userId);
    Task<CreateResponse<IdResponse>> CreateAsync(Guid contactId);
    Task<ReadResponse<PrivateRoomResponse>> GetPrivateRoomByIdAsync(Guid id);
    Task<ReadResponse<RoomLastSeenResponse>> GetRoomLastSeenAsync(Guid userId,Guid roomId);
}