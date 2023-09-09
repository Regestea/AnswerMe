using Models.Shared.RepositoriesResponseTypes;
using Models.Shared.Responses.Shared;

namespace AnswerMe.Application.Common.Interfaces;

public interface IGroupAdminRepository
{
    Task<ReadResponse<bool>> IsAdminAsync(Guid userId, Guid roomId);
    Task<ReadResponse<List<IdResponse>>> AdminListAsync(Guid roomId);
    Task<CreateResponse<IdResponse>> AddAsync(Guid userId, Guid roomId);
    Task<DeleteResponse> DeleteAsync(Guid userId, Guid roomId);
}