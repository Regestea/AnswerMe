using Azure.Data.Tables;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Options;
using ObjectStorage.Api.Entities;

namespace ObjectStorage.Api.Context
{
    public class BlobClientFactory : IBlobClientFactory
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly TableServiceClient _tableServiceClient;

        public BlobClientFactory(BlobServiceClient blobClient,TableServiceClient tableClient)
        {
            _blobServiceClient = blobClient;
            _tableServiceClient= tableClient;
        }
 
        public BlobContainerClient BlobStorageClient(ContainerName containerName)
        {
            BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(containerName.ToString());

            return containerClient;
        }

        public TableClient BlobTableClient(TableName tableName)
        {
            TableClient tableServiceClient = _tableServiceClient.GetTableClient(tableName.ToString());

            return tableServiceClient;
        }
    }
}
