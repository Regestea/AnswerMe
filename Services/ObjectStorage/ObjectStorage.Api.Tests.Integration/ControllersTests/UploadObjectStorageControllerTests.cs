using ObjectStorage.Api.Context;
using ObjectStorage.Api.Entities;

namespace ObjectStorage.Api.Tests.Integration.ControllersTests;


public class UploadObjectStorageControllerTests:IClassFixture<UploadApiFactory>
{
    
    private readonly HttpClient _client;
    // private readonly IBlobClientFactory _factory;

    public UploadObjectStorageControllerTests(UploadApiFactory uploadApiFactory)
    {
        _client = uploadApiFactory.CreateClient();
    }


    [Fact]
    public async Task TestBlobStorage()
    {
        // Create a new container
        _client.s
       
        // Upload a blob
        // var blobClient = blobContainerClient.GetBlobClient("test-blob.txt");
        // await using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("Hello Azurite!"));
        // await blobClient.UploadAsync(stream);
        // var isexist=await blobContainerClient.ExistsAsync();
        // Assert.True(isexist);
        // Verify the blob exists
        // bool exists = await blobClient.ExistsAsync();
        // Assert.True(exists);
    }
    
    
    [Fact]
    public void Create_UploadImageChunks_WhenFileIsValid()
    {
    //Arrange
    
    
    
    //Act
    //Assert
    }

    [Fact]
    public void Create_UploadVideoChunks_WhenFileIsValid()
    {
    }
}