using Microsoft.AspNetCore.Mvc.Testing;
using Models.Shared.Requests.ObjectStorage;
using Models.Shared.Responses.Shared;
using ObjectStorage.Api.Test.DataConvertor;
using Azure.Data.Tables;
using Azure.Storage.Blobs;
using IdentityServer.Shared.Client.DTOs;
using IdentityServer.Shared.Client.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Models.Shared.Responses.ObjectStorage;
using Moq;
using ObjectStorage.Api.Context;
using ObjectStorage.Api.Controllers;
using ObjectStorage.Api.Entities;
using ObjectStorage.Api.Services;
using ObjectStorage.Api.Test.Extensions;
using Xunit.Abstractions;

namespace ObjectStorage.Api.Test.Controllers
{
    public class ObjectStorageControllerTest
    {
        private const string BlobTestServer =
            "AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;DefaultEndpointsProtocol=http;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1;TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;";

        private readonly ITestOutputHelper _testOutputHelper;
        private readonly HttpClient _httpClient;

        public ObjectStorageControllerTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            var webAppFactory = new WebApplicationFactory<Program>();
            _httpClient = webAppFactory.CreateDefaultClient();
        }
        
        
        [Fact]
        public async Task Should_Upload_ProfileImage_ByChunk()
        {
            //Arrange
            var mockBlobClientFactory = new Mock<IBlobClientFactory>();
            mockBlobClientFactory.Setup(f => f.BlobStorageClient(ContainerName.profile))
                .Returns(() => new BlobContainerClient(BlobTestServer, ContainerName.profile.ToString()));
            
            mockBlobClientFactory.Setup(f => f.BlobTableClient(TableName.StashChunkDetail))
                .Returns(() => new TableClient(BlobTestServer, TableName.StashChunkDetail.ToString()));
            
            mockBlobClientFactory.Setup(f => f.BlobTableClient(TableName.IndexObjectFile))
                .Returns(() => new TableClient(BlobTestServer, TableName.IndexObjectFile.ToString()));

            var fakeToken = "this is a fake token";

            var mockJwtTokenRepository = new Mock<IJwtTokenRepository>();
            mockJwtTokenRepository.Setup(f => f.GetJwtToken())
                .Returns(() => fakeToken);

            mockJwtTokenRepository.Setup(f => f.ExtractUserDataFromToken(fakeToken))
                .Returns(() => new UserDto()
                {
                    id = new Guid("02dd9a9a-eac2-43be-90b8-d0201b80a31d"),
                    PhoneNumber = "123456789123",
                    IdName = "fakeId"
                });

            var fileUploadService = new FileUploadService(mockBlobClientFactory.Object);


            var controller = new ObjectStorageController(fileUploadService, mockBlobClientFactory.Object,
                mockJwtTokenRepository.Object, null!);
            
            // Get the directory of the assembly (test project)
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            
            // Navigate up to the project directory
            string projectDirectory = Path.GetFullPath(Path.Combine(baseDirectory, @"..\..\..\"));
            
            // Combine it with the relative path to your image file
            string filePath = Path.Combine(projectDirectory, "Images", "TestImage.jpg");


            FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            
            var chunks = await fileStream.ConvertStreamToChunksAsync(20);
            

            //Act

            var responseToken = await controller.RequestUploadProfileImageTokenAsync(new ProfileImageUploadRequest()
            {
                FileSizeMB = fileStream.Length.SizeMB(),
                FileFormat = "jpg"
            });
            var resultToken =(OkObjectResult) responseToken;

            var tokenResponse =(TokenResponse) resultToken.Value!;

            
            for (int i = 0; i < chunks.Count; i++)
            {
                var isLastChunk = chunks[i] == chunks.Last();
                var fileChunkRequest = new FileChunkRequest()
                {
                    Data = chunks[i],
                    CurrentChunk = i,
                    LastChunk =isLastChunk ,
                    UploadToken = new Guid(tokenResponse.Token)
                };
                
              var result= await controller.UploadChunkAsync(fileChunkRequest);
              
              
              //Assert
              if (isLastChunk)
              {
                  Assert.IsType<TokenResponse>(((OkObjectResult)result).Value);
              }
              else
              {
                  Assert.IsType<ChunkUploadResponse>(((ObjectResult)result).Value);
              }
            }
        }
    }
}