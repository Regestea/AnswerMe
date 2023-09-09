using AnswerMe.Application.DTOs;
using AnswerMe.Application.DTOs.Media;
using Models.Shared.RepositoriesResponseTypes;
using Models.Shared.Requests.User;
using Models.Shared.Responses.Media;
using Models.Shared.Responses.Shared;

namespace AnswerMe.Application.Common.Interfaces;

public interface IMediaRepositoryRRRRRRRRRRR
{
    Task<ReadResponse<MediaResponse>> GetMediaByIdAsync(Guid id);
    Task<CreateResponse<IdResponse>> AddMediaAsync(Guid userId, AddMediaDto request);
    Task<UpdateResponse> EditMediaAsync(Guid userId, Guid mediaId, EditMediaDto request);
    Task<DeleteResponse> DeleteMediaAsync(Guid userId, Guid mediaId);
}