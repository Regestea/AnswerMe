using Azure.Storage.Blobs.Models;
using Models.Shared.RepositoriesResponseTypes;
using Models.Shared.Requests.ObjectStorage;
using Models.Shared.Responses.ObjectStorage;
using ObjectStorage.Api.DTOs;
using ObjectStorage.Api.Entities;

namespace ObjectStorage.Api.Services.InterFaces;

public interface IFileUploadService
{
    public Task<UpdateResponse> UploadChunkAsync(FileChunkDto fileChunkDto, CancellationToken cancellationToken = default);

    public Task<DeleteResponse> DeleteObjectAsync(ContainerName containerName, string fileName);
}