using Azure.Storage.Blobs.Models;
using ObjectStorage.Api.Entities;

namespace ObjectStorage.Api.Services.InterFaces;

public interface IFileUploadService
{
    public Task<string?> UploadObjectAsync(ContainerName containerName, string fileName, Stream stream, AccessTier accessTier, CancellationToken cancellationToken = default);

    public Task<bool> DeleteObjectAsync(ContainerName containerName, string fileName);
}