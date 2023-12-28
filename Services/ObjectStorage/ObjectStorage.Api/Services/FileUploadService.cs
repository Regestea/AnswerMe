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
using Models.Shared.OneOfTypes;
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


        public async Task<UpdateResponse> UploadChunkAsync(FileChunkDto fileChunkDto,
            CancellationToken cancellationToken = default)
        {
            // Get the BlobClient
            var blobClient = _blobClientFactory.BlobStorageClient(fileChunkDto.ContainerName);

            var blockClient = blobClient.GetBlockBlobClient(fileChunkDto.FileName + "." + fileChunkDto.FileFormat);
            var blockId = Convert.ToBase64String(Encoding.UTF8.GetBytes(fileChunkDto.CurrentChunk.ToString("0000000")));
           
            await blockClient.StageBlockAsync(blockId, new MemoryStream(fileChunkDto.Data.ToArray()),
                cancellationToken: cancellationToken);


            if (cancellationToken.IsCancellationRequested)
            {
                return new Error<string>("Upload has been canceled");
            }

            return new Success();
        }

        public async Task<UpdateResponse> FinalizeUpload(FinalizeUploadDto finalizeUploadDto)
        {
            var blobClient =
                _blobClientFactory.BlobStorageClient(Enum.Parse<ContainerName>(finalizeUploadDto.ContainerName));

            var blockClient =
                blobClient.GetBlockBlobClient(finalizeUploadDto.FileName + "." + finalizeUploadDto.FileFormat);
            var blockIds = new List<string>();
            
            try
            {
                var blockList= await blockClient.GetBlockListAsync();
                foreach (var blobBlock in blockList.Value.UncommittedBlocks)
                {
                    blockIds.Add(blobBlock.Name);
                }
                
                await blockClient.CommitBlockListAsync(blockIds, null, null, null,
                    (AccessTier)finalizeUploadDto.AccessTier);
            }
            
            catch (Exception )
            {
                return new ValidationFailed() {Field = "",Error = "you can't finalize this upload file"};
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