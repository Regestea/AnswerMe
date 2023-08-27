using Azure.Storage.Blobs;
using Moq;
using ObjectStorage.Api.Context;
using ObjectStorage.Api.Entities;
using Azure.Storage.Blobs.Models;
using Models.Shared.RepositoriesResponseTypes;
using Models.Shared.Requests.ObjectStorage;
using ObjectStorage.Api.Services;
using Xunit.Abstractions;
using ObjectStorage.Api.Test.DataGenerator;
using ObjectStorage.Api.Test.DataConvertor;
using Models.Shared.Responses.ObjectStorage;
using ObjectStorage.Api.Extensions;

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
    public async Task UploadChunk_Should_UploadChunkToBlobStorage()
    {
        // Arrange
        var mockBlobClientFactory = new Mock<IBlobClientFactory>();

        mockBlobClientFactory.Setup(f => f.BlobStorageClient(ContainerName.image))
            .Returns(() => new BlobContainerClient(BlobTestServer, ContainerName.image.ToString()));

        var fileStream = TextToImageStream.ConvertTextToImageStream("test image");

        var fileUploadService = new FileUploadService(mockBlobClientFactory.Object);

        var fileName = Guid.NewGuid().ToString();

        var chunks = await fileStream.ConvertStreamToChunksAsync(1);

        // Act
        var aa = chunks.First().SizeMB();
        var bb = chunks.First().SizeMB();

        _testOutputHelper.WriteLine($"aa is {aa}");
        _testOutputHelper.WriteLine($"bb is {bb}");
        //for (int i = 0; i < chunks.Count; i++)
        //{
        //    var result = await fileUploadService.UploadChunkAsync(ContainerName.image, fileName, ".png", new FileChunkRequest()
        //    {
        //        Data = chunks[i],
        //        CurrentChunk = i,
        //        LastChunk = chunks[i] == chunks.Last()
        //        //LastChunk = i == chunks.Count-1
        //    }, AccessTier.Archive);
        //    // Assert
        //    Assert.NotNull(result);
        //    Assert.IsType<CreateResponse<UploadObjectResponse>>(result);
        //}



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

        //var uploadResult = await fileUploadService.UploadObjectAsync(ContainerName.image, "Test.png", fileStream, AccessTier.Archive,
        //    CancellationToken.None);

        var fileName = Guid.NewGuid().ToString();
        var fileFormat = ".png";

        await mockBlobClientFactory.Object.BlobStorageClient(ContainerName.image).UploadBlobAsync(fileName, fileStream);
        var deleteResult = await fileUploadService.DeleteObjectAsync(ContainerName.image, fileName + fileFormat);

        // Assert
        Assert.NotNull(deleteResult);
        Assert.IsType<DeleteResponse>(deleteResult);
    }

}