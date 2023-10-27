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

        public BlobClientFactory(IConfiguration configuration)
        {
            string blobConnectionString = configuration.GetSection("BlobStorage:BlobConnectionString").Value ??
                                          throw new InvalidOperationException();
            string tableConnectionString = configuration.GetSection("BlobStorage:BlobConnectionString").Value ??
                                          throw new InvalidOperationException();
            _blobServiceClient = new BlobServiceClient(blobConnectionString);
            _tableServiceClient= new TableServiceClient(tableConnectionString);
        }

        public BlobContainerClient BlobStorageClient(ContainerName containerName)
        {
            BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(containerName.ToString());

            return containerClient;
        }

        public TableClient BlobTableClient(TableName tableName)
        {
            TableClient tableClient = _tableServiceClient.GetTableClient(tableName.ToString());

            return tableClient;
        }
    }
}
