using Azure.Data.Tables;
using Azure.Storage.Blobs;
using ObjectStorage.Api.Entities;

namespace ObjectStorage.Api.Context;

public interface IBlobClientFactory
{
    BlobContainerClient BlobStorageClient(ContainerName containerName);
    TableClient BlobTableClient();
}