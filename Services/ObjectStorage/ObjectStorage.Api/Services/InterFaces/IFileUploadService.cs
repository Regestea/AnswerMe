using Azure.Storage.Blobs.Models;
using Models.Shared.RepositoriesResponseTypes;
using Models.Shared.Responses.ObjectStorage;
using ObjectStorage.Api.Entities;

namespace ObjectStorage.Api.Services.InterFaces;

public interface IFileUploadService
{
    public Task<CreateResponse<UploadObjectResponse>> UploadObjectAsync(ContainerName containerName, string fileName, Stream stream, AccessTier accessTier, CancellationToken cancellationToken = default);

    public Task<DeleteResponse> DeleteObjectAsync(ContainerName containerName, string fileName);
}