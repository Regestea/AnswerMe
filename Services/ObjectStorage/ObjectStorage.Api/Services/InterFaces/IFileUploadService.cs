using Azure.Storage.Blobs.Models;
using Models.Shared.RepositoriesResponseTypes;
using Models.Shared.Requests.ObjectStorage;
using Models.Shared.Responses.ObjectStorage;
using ObjectStorage.Api.DTOs;
using ObjectStorage.Api.Entities;

namespace ObjectStorage.Api.Services.InterFaces;

/// <summary>
/// Interface for file upload service.
/// </summary>
public interface IFileUploadService
{
    /// <summary>
    /// Uploads a file chunk asynchronously.
    /// </summary>
    /// <param name="fileChunkDto">The file chunk data to upload.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous upload operation.</returns>
    public Task<UpdateResponse> UploadChunkAsync(FileChunkDto fileChunkDto, CancellationToken cancellationToken = default);

    /// Deletes an object in the specified container asynchronously.
    /// @param containerName The name of the container where the object is stored.
    /// @param fileName The name of the object to be deleted.
    /// @return A task that represents the asynchronous delete operation.
    /// The task result contains a DeleteResponse object with the status and response
    /// properties.
    /// /
    public Task<DeleteResponse> DeleteObjectAsync(ContainerName containerName, string fileName);
}