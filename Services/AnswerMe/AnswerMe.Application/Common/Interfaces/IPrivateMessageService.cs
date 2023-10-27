using Models.Shared.RepositoriesResponseTypes;
using Models.Shared.Requests.Message;
using Models.Shared.Requests.Shared;
using Models.Shared.Responses.Message;
using Models.Shared.Responses.Shared;

namespace AnswerMe.Application.Common.Interfaces;

public interface IPrivateMessageService
{
    Task<CreateResponse<IdResponse>> SendAsync(Guid loggedInUserId, Guid roomId, SendMessageRequest request);
    Task<ReadResponse<PagedListResponse<MessageResponse>>> GetListAsync(Guid loggedInUserId, Guid roomId, PaginationRequest request);
    Task<UpdateResponse> UpdateAsync(Guid loggedInUserId, Guid messageId, EditMessageRequest request);
    Task<UpdateResponse> UpdateMediaAsync(Guid loggedInUserId, Guid messageId, Guid mediaId, EditMessageMediaRequest request);
    Task<DeleteResponse> DeleteAsync(Guid loggedInUserId, Guid messageId);
    Task<DeleteResponse> DeleteMediaAsync(Guid loggedInUserId, Guid messageId, Guid mediaId);

}
