using Azure.Storage.Blobs.Specialized;
using ObjectStorage.Api.Context;
using ObjectStorage.Api.Entities;
using ObjectStorage.Api.Services.InterFaces;
using System.Text;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using Microsoft.Azure.Cosmos.Serialization.HybridRow;
using Models.Shared.RepositoriesResponseTypes;
using Models.Shared.Requests.ObjectStorage;
using Models.Shared.Responses.ObjectStorage;
using OneOf.Types;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Net;
using ObjectStorage.Api.DTOs;

namespace ObjectStorage.Api.Services
{
    public class FileUploadService : IFileUploadService
    {
        private readonly IBlobClientFactory _blobClientFactory;

        public FileUploadService(IBlobClientFactory blobClientFactory)
        {
            _blobClientFactory = blobClientFactory;
        }


        public async Task<UpdateResponse> UploadChunkAsync(FileChunkDto fileChunkDto, CancellationToken cancellationToken = default)
        {

            // Get the BlobClient
            var blobClient = _blobClientFactory.BlobStorageClient(fileChunkDto.ContainerName);

            var blockClient = blobClient.GetBlockBlobClient(fileChunkDto.FileName + "." + fileChunkDto.FileFormat);
            var blockId = Convert.ToBase64String(Encoding.UTF8.GetBytes(fileChunkDto.CurrentChunk.ToString("0000000")));

            await blockClient.StageBlockAsync(blockId, new MemoryStream(fileChunkDto.Data), cancellationToken: cancellationToken);

            if (fileChunkDto.LastChunk)
            {
                var blockIds = new List<string>();
                for (int i = 0; i <= fileChunkDto.TotalChunks; i++)
                {
                    blockIds.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes(i.ToString("0000000"))));
                }
                await blockClient.CommitBlockListAsync(blockIds, null, null, null, fileChunkDto.AccessTier, cancellationToken);

            }

            if (cancellationToken.IsCancellationRequested)
            {
                return new Error<string>("Upload has been canceled");
            }

            return new Success();
        }

        public async Task<DeleteResponse> DeleteObjectAsync(ContainerName containerName, string fileName)
        {
            await _blobClientFactory.BlobStorageClient(containerName)
                .DeleteBlobIfExistsAsync(fileName);
            return new Success();
        }
    }
}
