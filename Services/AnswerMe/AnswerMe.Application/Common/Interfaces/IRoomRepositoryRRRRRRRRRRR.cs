using AnswerMe.Application.DTOs.Room;
using AnswerMe.Domain.Entities;
using Models.Shared.RepositoriesResponseTypes;
using Models.Shared.Requests.Room;
using Models.Shared.Responses.Shared;

namespace AnswerMe.Application.Common.Interfaces;

public interface IRoomRepositoryRRRRRRRRRRR
{
    Task<ReadResponse<bool>> ExistRoomAsync(Guid id);
    Task<CreateResponse<IdResponse>> AddAsync(AddRoomDto addRoomDto);
    Task<UpdateResponse> EditAsync(Guid roomId, Guid userId, EditRoomRequest request);
}