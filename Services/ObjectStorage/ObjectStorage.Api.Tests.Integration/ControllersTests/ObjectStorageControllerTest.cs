using Azure.Data.Tables;
using Azure.Storage.Blobs;
using IdentityServer.Shared.Client.DTOs;
using IdentityServer.Shared.Client.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Models.Shared.Requests.Shared;
using Models.Shared.Requests.Upload;
using Models.Shared.Responses.ObjectStorage;
using Models.Shared.Responses.Shared;
using Moq;
using ObjectStorage.Api.Context;
using ObjectStorage.Api.Controllers;
using ObjectStorage.Api.Entities;
using ObjectStorage.Api.Services;
using ObjectStorage.Api.Tests.Integration.Extensions;
using Xunit.Abstractions;

namespace ObjectStorage.Api.Tests.Integration.ControllersTests
{
    public class ObjectStorageControllerTest
    {
        private const string BlobTestServer =
            "AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;DefaultEndpointsProtocol=http;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1;TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;";

        private readonly ITestOutputHelper _testOutputHelper;
       
        public ObjectStorageControllerTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
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
            
            var dataConvertor = new DataConvertor();
            
            var numberOfChunks = 20;
            
            await using var fs = fileStream;
            
            
            //Act
            
            var responseToken = await controller.RequestUploadTokenAsync(new UploadRequest()
            {
                FileType = FileType.profile,
                FileSizeMb = fileStream.Length.SizeMB(),
                FileFormat = "jpg"
            });
            var resultToken =(OkObjectResult) responseToken;
            
            var tokenResponse =(TokenResponse) resultToken.Value!;
            
            await foreach (var chunk in dataConvertor.GetStreamChunksAsync(fs, numberOfChunks))
            {
                var fileChunkRequest = new FileChunkRequest()
                {
                    Data =chunk.ToArray(),
                    UploadToken = tokenResponse.Token
                };
            
                var uploadResult = await controller.UploadChunkAsync(fileChunkRequest);
            
            
                //Assert
            
                var chunkUploadObjectResult = (ObjectResult)uploadResult;
                var chunkUploadResult =(ChunkUploadResponse)chunkUploadObjectResult.Value!;
            
                Assert.NotNull(chunkUploadResult);
                Assert.NotNull(chunkUploadObjectResult);
                Assert.IsType<ObjectResult>(uploadResult);
                Assert.IsType<ChunkUploadResponse>(chunkUploadObjectResult.Value);
                _testOutputHelper.WriteLine("Total Uploaded Chunks : " + (chunkUploadResult.TotalUploadedChunks));
                _testOutputHelper.WriteLine("Total Uploaded Size MB : " + chunkUploadResult.TotalUploadedSizeMB);
            }
            
            var finalizeResult= await controller.FinalizeUpload(new TokenRequest(){Token = tokenResponse.Token});
            
            //Assert
            
            var finalizeUploadObjectResult = (ObjectResult)finalizeResult;
            var finalizeUploadResult =(TokenResponse)finalizeUploadObjectResult.Value!;
            
            Assert.NotNull(finalizeUploadResult);
            Assert.NotNull(finalizeUploadObjectResult);
            Assert.IsType<OkObjectResult>(finalizeResult);
            Assert.IsType<TokenResponse>(finalizeUploadObjectResult.Value);
            
            _testOutputHelper.WriteLine("Uploaded File Token : "+ finalizeUploadResult.Token);
            
        }
    }
}