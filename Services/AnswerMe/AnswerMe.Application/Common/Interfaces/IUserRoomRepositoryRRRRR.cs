using Models.Shared.RepositoriesResponseTypes;
using Models.Shared.Responses.Shared;

namespace AnswerMe.Application.Common.Interfaces;

public interface IUserRoomRepositoryRRRRR
{
    Task<CreateResponse<IdResponse>> AddAsync(Guid userId, Guid roomId);
    Task<DeleteResponse> DeleteAsync(Guid userId,Guid roomId);
    Task<ReadResponse<List<IdResponse>>> UserRoomListAsync(Guid userId);
    Task<ReadResponse<List<IdResponse>>> RoomUserListAsync(Guid roomId);
}