//using Azure.Storage.Blobs;
//using Moq;
//using ObjectStorage.Api.Context;
//using ObjectStorage.Api.Entities;
//using Azure.Storage.Blobs.Models;
//using Models.Shared.RepositoriesResponseTypes;
//using Models.Shared.Requests.ObjectStorage;
//using ObjectStorage.Api.Services;
//using Xunit.Abstractions;
//using ObjectStorage.Api.Test.DataGenerator;
//using Models.Shared.Responses.ObjectStorage;
//using ObjectStorage.Api.DTOs;
//using ObjectStorage.Api.Extensions;

//namespace ObjectStorage.Api.Test.Services;

//public class FileUploadServiceTest
//{
//    private const string BlobTestServer = "AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;DefaultEndpointsProtocol=http;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1;TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;";
//    private readonly ITestOutputHelper _testOutputHelper;

//    public FileUploadServiceTest(ITestOutputHelper testOutputHelper)
//    {
//        _testOutputHelper = testOutputHelper;
//    }

//    [Fact]
//    public async Task UploadChunk_Should_UploadChunkToBlobStorage()
//    {
//        // Arrange
//        var mockBlobClientFactory = new Mock<IBlobClientFactory>();

//        mockBlobClientFactory.Setup(f => f.BlobStorageClient(ContainerName.image))
//            .Returns(() => new BlobContainerClient(BlobTestServer, ContainerName.image.ToString()));

//        var fileStream = TextToImageStream.ConvertTextToImageStream("test image");
//        var chunks = await fileStream.ConvertStreamToChunksAsync(2);

//        var fileUploadService = new FileUploadService(mockBlobClientFactory.Object);

//        var fileName = Guid.NewGuid().ToString();

//        // Act

//        for (int i = 0; i < chunks.Count; i++)
//        {
//            var fileChunkDto = new FileChunkDto()
//            {
//                Data = chunks[i],
//                CurrentChunk = i,
//                AccessTier = AccessTier.Archive.ToString(),
//                ContainerName = ContainerName.image,
//                FileFormat = ".png",
//                FileName = fileName,
//                TotalUploadedChunks = chunks.Count,
//            };
//            var result = await fileUploadService.UploadChunkAsync(fileChunkDto);

//            // Assert
//            Assert.NotNull(result);
//            Assert.IsType<UpdateResponse>(result);
//            Assert.True(result.IsSuccess);
//        }



//    }

//    [Fact]
//    public async Task Delete_Should_DeleteObjectFromBlobStorage()
//    {
//        // Arrange
//        var mockBlobClientFactory = new Mock<IBlobClientFactory>();

//        mockBlobClientFactory.Setup(f => f.BlobStorageClient(ContainerName.image))
//            .Returns(() => new BlobContainerClient(BlobTestServer, ContainerName.image.ToString()));

//        var fileStream = TextToImageStream.ConvertTextToImageStream("test image");

//        var fileUploadService = new FileUploadService(mockBlobClientFactory.Object);

//        // Act

//        var fileName = Guid.NewGuid().ToString();
//        var fileFormat = ".png";

//        await mockBlobClientFactory.Object.BlobStorageClient(ContainerName.image).UploadBlobAsync(fileName, fileStream);
//        var deleteResult = await fileUploadService.DeleteObjectAsync(ContainerName.image, fileName + fileFormat);

//        // Assert
//        Assert.NotNull(deleteResult);
//        Assert.IsType<DeleteResponse>(deleteResult);
//    }

//}