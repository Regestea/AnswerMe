using Models.Shared.RepositoriesResponseTypes;
using Models.Shared.Requests.Shared;
using Models.Shared.Requests.Upload;
using Models.Shared.Responses.ObjectStorage;
using Models.Shared.Responses.Shared;

namespace AnswerMe.Client.Core.Services.Interfaces;

public interface IObjectStorageService
{
    Task<CreateResponse<ChunkUploadResponse>> UploadChunkAsync(FileChunkRequest request);
    Task<CreateResponse<TokenResponse>> FinalizeUploadAsync(TokenRequest request);
    Task<CreateResponse<TokenResponse>> GetUploadTokenAsync(UploadRequest request);

}