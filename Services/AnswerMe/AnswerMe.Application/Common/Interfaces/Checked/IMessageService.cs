using Models.Shared.RepositoriesResponseTypes;
using Models.Shared.Requests.Message;
using Models.Shared.Requests.Shared;
using Models.Shared.Responses.Message;
using Models.Shared.Responses.Shared;

namespace AnswerMe.Application.Common.Interfaces.Checked;

public interface IMessageService
{
    Task<CreateResponse<IdResponse>> SendPrivateMessage(Guid userId, Guid roomId, SendMessageRequest request);
    Task<CreateResponse<IdResponse>> SendGroupMessage(Guid userId, Guid groupId, SendMessageRequest request);
    Task<ReadResponse<List<MessageResponse>>> GetPrivateMessages(Guid userId, Guid roomId, PaginationRequest request);
    Task<ReadResponse<List<MessageResponse>>> GetGroupMessages(Guid userId, Guid roomId, PaginationRequest request);
    Task<UpdateResponse> UpdateMessage(Guid userId, Guid messageId, UpdateMessageRequest request);
    Task<UpdateResponse> UpdateMessageMedia(Guid userId, Guid messageId, UpdateMessageMediaRequest request);
    Task<DeleteResponse> DeleteMessage(Guid userId, Guid messageId);
    Task<DeleteResponse> DeleteMessageMedia(Guid userId, Guid messageId, Guid mediaId);

}