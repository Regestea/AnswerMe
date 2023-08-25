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

namespace ObjectStorage.Api.Services
{
    public class FileUploadService : IFileUploadService
    {
        private readonly IBlobClientFactory _blobClientFactory;

        public FileUploadService(IBlobClientFactory blobClientFactory)
        {
            _blobClientFactory = blobClientFactory;
        }

        public async Task<CreateResponse<UploadObjectResponse>> UploadObjectAsync(ContainerName containerName, string fileName, Stream stream,
            AccessTier accessTier, CancellationToken cancellationToken = default)
        {

            string blobFileName = Guid.NewGuid().ToString();
            string fileFormat = Path.GetExtension(fileName);
            // Get the BlobClient and block size
            var blobClient = _blobClientFactory.BlobStorageClient(containerName);
            var blockClient = blobClient.GetBlockBlobClient(blobFileName + fileFormat);

            var blockSize = 4 * 1024 * 1024;

            // Create a list to track the block IDs
            var blockIds = new List<string>();

            // Read the stream in chunks and upload each chunk to Azure Blob Storage
            var buffer = new byte[blockSize];
            int bytesRead;
            int blockNumber = 0;
            long fileSize = stream.Length;

            while ((bytesRead = await stream.ReadAsync(buffer, 0, blockSize, cancellationToken)) > 0)
            {
                // Increment the block number and create a new block ID
                blockNumber++;
                var blockId = Convert.ToBase64String(Encoding.UTF8.GetBytes(blockNumber.ToString("0000000")));

                // Upload the block to Azure Blob Storage
                await blockClient.StageBlockAsync(blockId, new MemoryStream(buffer, 0, bytesRead), null, null, null, cancellationToken);

                blockIds.Add(blockId);
            }

            // Commit the block list to Azure Blob Storage
            var response = await blockClient.CommitBlockListAsync(blockIds, null, null, null, accessTier, cancellationToken);

            if (cancellationToken.IsCancellationRequested)
            {
                return new Error<string>("Upload has been canceled");
            }

            return new Success<UploadObjectResponse>(new UploadObjectResponse() { RowKey = blobFileName, FileFormat = fileFormat });
        }

        public async Task<CreateResponse<UploadObjectResponse>> UploadChunkAsync(ContainerName containerName, string? fileName,string fileFormat, FileChunkRequest chunkRequest,
            AccessTier accessTier, CancellationToken cancellationToken = default)
        {
            
            // Get the BlobClient
            var blobClient = _blobClientFactory.BlobStorageClient(containerName);

            if (string.IsNullOrEmpty(fileName))
            {
                fileName=Guid.NewGuid().ToString();

            }

            var blockClient = blobClient.GetBlockBlobClient(fileName + fileFormat);
            var blockId = Convert.ToBase64String(Encoding.UTF8.GetBytes(chunkRequest.ChunkNumber.ToString("0000000")));

            await blockClient.StageBlockAsync(blockId,new MemoryStream(chunkRequest.Data), cancellationToken: cancellationToken);

            if (chunkRequest.LastChunk)
            {
                var blockIds = new List<string>();
                for (int i = 0; i <= chunkRequest.ChunkNumber; i++)
                {
                    blockIds.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes(i.ToString("0000000"))));
                }
                var response = await blockClient.CommitBlockListAsync(blockIds, null, null, null, accessTier, cancellationToken);
            }

            if (cancellationToken.IsCancellationRequested)
            {
                return new Error<string>("Upload has been canceled");
            }

            return new Success<UploadObjectResponse>(new UploadObjectResponse() { RowKey = fileName, FileFormat = fileFormat });
        }

        public async Task<DeleteResponse> DeleteObjectAsync(ContainerName containerName, string fileName)
        {
            await _blobClientFactory.BlobStorageClient(containerName)
                .DeleteBlobIfExistsAsync(fileName);
            return new Success();
        }
    }
}
