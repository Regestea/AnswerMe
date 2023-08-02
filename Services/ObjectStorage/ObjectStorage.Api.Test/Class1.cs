using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ObjectStorage.Api.Context;
using ObjectStorage.Api.Controllers;
using ObjectStorage.Api.Entities;

namespace ObjectStorage.Api.Test;

public class ObjectStorageControllerTests
{
    private const string BlobTestServer= "AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;DefaultEndpointsProtocol=http;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1;TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;";
    
    [Fact]
    public async Task Upload_ShouldUploadStreamToBlobStorage()
    {
        var mockBlobClientFactory = new Mock<IBlobClientFactory>();

        mockBlobClientFactory.Setup(f => f.BlobStorageClient(ContainerName.image))
            .Returns(() => new BlobContainerClient(BlobTestServer, ContainerName.image.ToString()));
        var fileStream = TextToImageStream.ConvertTextToImageStream("test image");
        
        var controller = new ObjectStorageController(mockBlobClientFactory.Object);

        var result = await controller.Upload(fileStream, "test.png");

        Assert.NotNull(result);
        Assert.IsType<OkResult>(result);
        

        //// Arrange
        //var stream = new MemoryStream(new byte[] { 0x00, 0x01, 0x02 });
        //var fileName = "test.png";


        //var mockBlockBlobClient = new Mock<IBlockBlobClient>();
        //mockBlobClient.Setup(c => c.GetBlockBlobClient(It.IsAny<string>()));

        //var mockBlockBlobClientSetup = mockBlockBlobClient.Setup(c => c.StageBlockAsync(It.IsAny<string>(), It.IsAny<Stream>(), null));
        //mockBlockBlobClientSetup.Callback<string, Stream, byte[]>((blockId, blockData, _) =>
        //{
        //    // Assert
        //    Assert.NotNull(blockId);
        //    Assert.Equal(3, blockData.Length);
        //    Assert.Equal(0x00, blockData[0]);
        //    Assert.Equal(0x01, blockData[1]);
        //    Assert.Equal(0x02, blockData[2]);
        //});

        //mockBlockBlobClient.Setup(c => c.CommitBlockListAsync(It.IsAny<IEnumerable<string>>()));

        //var controller = new ObjectStorageController(mockBlobClientFactory.Object);

        //// Act
        //var result = await controller.Upload(stream, fileName);

        //// Assert
        //Assert.IsType<OkResult>(result);
        //mockBlobClientFactory.Verify(f => f.BlobStorageClient(ContainerName.image), Times.Once);
        //mockBlobClient.Verify(c => c.GetBlockBlobClient(It.IsAny<string>()), Times.Once);
        //mockBlockBlobClient.Verify(c => c.StageBlockAsync(It.IsAny<string>(), It.IsAny<Stream>(), null), Times.Once);
        //mockBlockBlobClient.Verify(c => c.CommitBlockListAsync(It.IsAny<IEnumerable<string>>()), Times.Once);
    }
}