using Azure.Storage.Blobs;
using Moq;
using ObjectStorage.Api.Context;
using ObjectStorage.Api.Entities;
using Azure.Storage.Blobs.Models;
using Models.Shared.RepositoriesResponseTypes;
using ObjectStorage.Api.Services;
using Xunit.Abstractions;
using ObjectStorage.Api.Test.DataGenerator;
using Models.Shared.Responses.ObjectStorage;

namespace ObjectStorage.Api.Test.Services;

public class FileUploadServiceTest
{
    private const string BlobTestServer = "AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;DefaultEndpointsProtocol=http;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1;TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;";
    private readonly ITestOutputHelper _testOutputHelper;

    public FileUploadServiceTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }


    [Fact]
    public async Task Upload_Should_UploadStreamToBlobStorage()
    {
        // Arrange
        var mockBlobClientFactory = new Mock<IBlobClientFactory>();

        mockBlobClientFactory.Setup(f => f.BlobStorageClient(ContainerName.image))
            .Returns(() => new BlobContainerClient(BlobTestServer, ContainerName.image.ToString()));

        var fileStream = TextToImageStream.ConvertTextToImageStream("test image");
       
        var fileUploadService = new FileUploadService(mockBlobClientFactory.Object);

        // Act

        var result = await fileUploadService.UploadObjectAsync(ContainerName.image, "Test.png", fileStream, AccessTier.Archive,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<CreateResponse<UploadObjectResponse>>(result);

        // Clean up
        var blobClient = new BlobContainerClient(BlobTestServer, ContainerName.image.ToString());
        await blobClient.DeleteBlobIfExistsAsync(result.AsT0.Value.RowKey + result.AsT0.Value.FileFormat);
    }

    [Fact]
    public async Task Delete_Should_DeleteObjectFromBlobStorage()
    {
        // Arrange
        var mockBlobClientFactory = new Mock<IBlobClientFactory>();

        mockBlobClientFactory.Setup(f => f.BlobStorageClient(ContainerName.image))
            .Returns(() => new BlobContainerClient(BlobTestServer, ContainerName.image.ToString()));

        var fileStream = TextToImageStream.ConvertTextToImageStream("test image");

        var fileUploadService = new FileUploadService(mockBlobClientFactory.Object);

        // Act

        var uploadResult = await fileUploadService.UploadObjectAsync(ContainerName.image, "Test.png", fileStream, AccessTier.Archive,
            CancellationToken.None);
        var deleteResult = await fileUploadService.DeleteObjectAsync(ContainerName.image, uploadResult.AsT0.Value.RowKey+uploadResult.AsT0.Value.FileFormat);

        // Assert
        Assert.NotNull(uploadResult);
        Assert.IsType<CreateResponse<UploadObjectResponse>>(uploadResult);
        Assert.NotNull(deleteResult);
        Assert.IsType<DeleteResponse>(deleteResult);
    }

}