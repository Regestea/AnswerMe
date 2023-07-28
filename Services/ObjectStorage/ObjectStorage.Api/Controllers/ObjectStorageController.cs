using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ObjectStorage.Api.Context;
using ObjectStorage.Api.Entities;
using System.Reflection.Metadata;
using System.Text;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using static System.Reflection.Metadata.BlobBuilder;
using System.Drawing;
using System.IO;


namespace ObjectStorage.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ObjectStorageController : ControllerBase
    {
        private readonly IBlobClientFactory _blobClientFactory;

        public ObjectStorageController(IBlobClientFactory blobClientFactory)
        {
            _blobClientFactory = blobClientFactory;
        }

        [HttpPost]
        public async Task<IActionResult> Test(IFormFile file)
        {
            // Get the BlobClient and block size
            var blobClient = _blobClientFactory.BlobStorageClient(ContainerName.image);
            var blockClient = blobClient.GetBlockBlobClient(Guid.NewGuid().ToString() + Path.GetExtension(file.FileName));
            var blockSize = 1 * 1024 * 1024;

            // Create a list to track the block IDs
            var blockIds = new List<string>();

            // Open a stream to the file
            using var stream = file.OpenReadStream();

            // Read the file in chunks and upload each chunk to Azure Blob Storage
            var buffer = new byte[blockSize];
            int bytesRead;
            int blockNumber = 0;
            long fileSize = file.Length;

            while ((bytesRead = await stream.ReadAsync(buffer, 0, blockSize)) > 0)
            {
                // Increment the block number and create a new block ID
                blockNumber++;
                var blockId = Convert.ToBase64String(Encoding.UTF8.GetBytes(blockNumber.ToString("0000000")));

                // Upload the block to Azure Blob Storage
                await blockClient.StageBlockAsync(blockId, new MemoryStream(buffer, 0, bytesRead), null);
                Console.WriteLine(blockId);
                blockIds.Add(blockId);

                // Calculate and output the percentage of uploaded data
                var uploadedSize = blockNumber * blockSize;
                var percentage = (double)uploadedSize / fileSize * 100;
                Console.WriteLine($"Uploaded {percentage:f0}% of the file.");
            }

            // Commit the block list to Azure Blob Storage
            await blockClient.CommitBlockListAsync(blockIds);

            return Ok();
        }
    }
}
