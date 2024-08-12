using AnswerMe.Client.Core.Services;
using AnswerMe.Client.Core.Services.Interfaces;
using Microsoft.AspNetCore.Components.Forms;
using Models.Shared.Interfaces;
using Models.Shared.OneOfTypes;
using Models.Shared.RepositoriesResponseTypes;
using Models.Shared.Requests.Shared;
using Models.Shared.Requests.Upload;
using Models.Shared.Responses.Shared;

namespace AnswerMe.Client.Core.Extensions;

public static class UploadExtension
{
    public static async Task<CreateResponse<TokenResponse>> UploadChunks(this IBrowserFile file,IObjectHelperService objectHelperService, IObjectStorageService storageService, string uploadToken,Guid fileId, Action stateHasChanged, Action<int?> uploadPercentage, bool uploadCancel)
    {
        var chunksCount = objectHelperService.CalculateChunkCount(file.Size.SizeMB());
        var chunkPercentage = 100 / chunksCount;
        var maxFileSize = unchecked(1024 * 1024 * 2000);
        await using var filestream = file.OpenReadStream(maxFileSize);
        uploadPercentage.Invoke(0);
        stateHasChanged.Invoke();

        await foreach (var chunk in objectHelperService.GetStreamChunksAsync(filestream, chunksCount))
        {
            if (uploadCancel)
            {
                break;
            }
            var fileChunkRequest = new FileChunkRequest()
            {
                Data = chunk.ToArray(),
                UploadToken = uploadToken
            };

            var chunkUploadResponse = await storageService.UploadChunkAsync(fileChunkRequest);
            
            var uploadProgress = (chunkUploadResponse.AsSuccess.Value.TotalUploadedChunks) * chunkPercentage;
            uploadPercentage.Invoke(uploadProgress);
            stateHasChanged.Invoke();
            
        }
        
        if (!uploadCancel)
        {
            var fileTokenResponse = await storageService.FinalizeUploadAsync(new TokenRequest(){Token = uploadToken});
            uploadPercentage.Invoke(null);
            stateHasChanged.Invoke();
            
            return fileTokenResponse;

        }

        return new ValidationFailed() { Field = "",Error = "upload canceled"};
    }
}