using Models.Shared.RepositoriesResponseTypes;
using Models.Shared.Requests.Message;
using Models.Shared.Requests.Shared;
using Models.Shared.Responses.Message;
using Models.Shared.Responses.Shared;

namespace AnswerMe.Client.Core.Services.Interfaces;

public interface IGroupMessageService
{
    Task<ReadResponse<PagedListResponse<MessageResponse>>> GetGroupMessagesAsync(Guid roomId, bool jumpToUnRead, PaginationRequest request);
    Task<CreateResponse<IdResponse>> SendMessageAsync(Guid roomId, SendMessageRequest request);
    Task<UpdateResponse> EditMessageTextAsync(Guid messageId, EditMessageRequest request);
    Task<DeleteResponse> DeleteMessageAsync(Guid messageId);
    Task<UpdateResponse> EditMessageMediaAsync(Guid messageId, Guid mediaId, EditMessageMediaRequest request);
    Task<DeleteResponse> DeleteMessageMediaAsync(Guid messageId, Guid mediaId);
}