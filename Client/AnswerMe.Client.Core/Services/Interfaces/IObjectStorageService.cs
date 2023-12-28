using Models.Shared.RepositoriesResponseTypes;
using Models.Shared.Requests.ObjectStorage;
using Models.Shared.Responses.ObjectStorage;
using Models.Shared.Responses.Shared;

namespace AnswerMe.Client.Core.Services.Interfaces;

public interface IObjectStorageService
{
    Task<CreateResponse<ChunkUploadResponse>> UploadChunkAsync(FileChunkRequest request);
    Task<CreateResponse<TokenResponse>> FinalizeUploadAsync(string uploadToken);
    Task<CreateResponse<TokenResponse>> GetUploadTokenAsync(ProfileImageUploadRequest request);
    Task<CreateResponse<TokenResponse>> GetUploadTokenAsync(ImageUploadRequest request);
    Task<CreateResponse<TokenResponse>> GetUploadTokenAsync(AudioUploadRequest request);
    Task<CreateResponse<TokenResponse>> GetUploadTokenAsync(VideoUploadRequest request);
    Task<CreateResponse<TokenResponse>> GetUploadTokenAsync(OtherUploadRequest request);
}