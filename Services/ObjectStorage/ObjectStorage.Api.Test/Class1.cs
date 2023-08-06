using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ObjectStorage.Api.Context;
using ObjectStorage.Api.Controllers;
using ObjectStorage.Api.Entities;
using ObjectStorage.Api.Services.InterFaces;
using System.IO;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Azure;
using ObjectStorage.Api.Services;
using Xunit.Abstractions;

namespace ObjectStorage.Api.Test;

public class ObjectStorageControllerTests
{
    private const string BlobTestServer = "AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;DefaultEndpointsProtocol=http;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1;TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;";
    private readonly ITestOutputHelper _testOutputHelper;

    public ObjectStorageControllerTests(ITestOutputHelper testOutputHelper)
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
        
        
        _testOutputHelper.WriteLine($"This is stream length: {(double)fileStream.Length / 1024} KB");

        // Act

        var result = await fileUploadService.UploadObjectAsync(ContainerName.image, "Test.png", fileStream, AccessTier.Archive,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<string>(result);

        // Clean up
        var blobClient = new BlobContainerClient(BlobTestServer, ContainerName.image.ToString());
        await blobClient.DeleteBlobIfExistsAsync(result);
    }
}