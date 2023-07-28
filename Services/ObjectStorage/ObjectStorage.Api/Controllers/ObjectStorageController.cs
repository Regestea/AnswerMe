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
            //await _blobClientFactory.BlobStorageClient(ContainerName.image).CreateIfNotExistsAsync(PublicAccessType.Blob);

            string fileExtension = Path.GetExtension(file.FileName);

            //// remove the dot from the file extension (e.g. "jpg")
            string fileFormat = fileExtension.Substring(1);

            var fileName = Guid.NewGuid() + $".{fileFormat}";

            //BlobClient client = _blobClientFactory.BlobStorageClient(ContainerName.image).GetBlobClient(fileName);



            //await using (Stream? data = file.OpenReadStream())
            //{
            //    // Upload the file async
            //    await client.UploadAsync(data);
            //}

            //await _blobClientFactory.BlobTableClient(TableName.image).CreateIfNotExistsAsync();

            //var tableClient =
            //    await _blobClientFactory.BlobTableClient(TableName.image).AddEntityAsync(new ObjectFile
            //    {
            //        RowKey = fileName,
            //        PartitionKey = ContainerName.image.ToString(),
            //        UserId = Guid.NewGuid(),
            //        FileName = fileName,
            //        ContainerName = ContainerName.image,
            //        FileFormat = fileFormat,
            //        HaveUse = false,
            //        Timestamp = DateTimeOffset.Now,
            //        Token = "some thing token"

            //    });


            var blockClient = _blobClientFactory.BlobStorageClient(ContainerName.image).GetBlockBlobClient(fileName);

            // local variable to track the current number of bytes read into buffer
            int bytesRead = 1024*1024;

            // track the current block number as the code iterates through the file
            int blockNumber = 0;

            int size = (int)(file.Length / 20)+1;

            // Create list to track blockIds, it will be needed after the loop
            List<string> blockList = new List<string>();

            do
            {
                // increment block number by 1 each iteration
                blockNumber++;

                // set block ID as a string and convert it to Base64 which is the required format
                string blockId = $"{blockNumber:0000000}";
                string base64BlockId = Convert.ToBase64String(Encoding.UTF8.GetBytes(blockId));

                // create buffer and retrieve chunk
                byte[] buffer = new byte[size];

                var stream = file.OpenReadStream();

                bytesRead = await stream.ReadAsync(buffer, 0, size);



                // Upload buffer chunk to Azure
                //await blob.PutBlockAsync(base64BlockId, new MemoryStream(buffer, 0, bytesRead), null);
                await blockClient.StageBlockAsync(base64BlockId, new MemoryStream(buffer, 0, bytesRead), null);
                // add the current blockId into our list
                blockList.Add(base64BlockId);

                // While bytesRead == size it means there is more data left to read and process

              

                long uploadedSize = blockNumber * size;
                double percentage = (double)uploadedSize / file.Length;
                Console.WriteLine($"Uploaded {percentage}% of the file.");



            } while (bytesRead <= size);

            // add the blockList to the Azure which allows the resource to stick together the chunks
            await blockClient.CommitBlockListAsync(blockList);


            return Ok();
        }
    }
}
