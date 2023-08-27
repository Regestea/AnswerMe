using Microsoft.AspNetCore.Mvc;
using ObjectStorage.Api.Context;
using ObjectStorage.Api.Entities;
using Azure.Storage.Blobs.Models;
using System.Net;
using IdentityServer.Shared.Client.Attributes;
using IdentityServer.Shared.Client.Repositories.Interfaces;
using Models.Shared.Requests.ObjectStorage;
using Models.Shared.Responses.Shared;
using ObjectStorage.Api.Services.InterFaces;
using Security.Shared.Extensions;
using Azure.Core;
using System.Threading;
using Azure;
using Azure.Data.Tables;
using IdentityServer.Shared.Client.DTOs;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using Models.Shared.Responses.ObjectStorage;
using ObjectStorage.Api.DTOs;
using ObjectStorage.Api.Extensions;

namespace ObjectStorage.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class ObjectStorageController : ControllerBase
    {
        private readonly IFileUploadService _fileUploadService;
        private readonly IBlobClientFactory _blobClientFactory;
        private readonly IJwtTokenRepository _jwtTokenRepository;

        public ObjectStorageController(IFileUploadService fileUploadService, IBlobClientFactory blobClientFactory, IJwtTokenRepository jwtTokenRepository)
        {
            _fileUploadService = fileUploadService;
            _blobClientFactory = blobClientFactory;
            _jwtTokenRepository = jwtTokenRepository;
        }


        [HttpPost]
        //[AuthorizeByIdentityServer]
        public async Task<IActionResult> UploadChunk([FromBody] FileChunkRequest request, CancellationToken cancellationToken = default)
        {
            //var requestToken = _jwtTokenRepository.GetJwtToken();
            //var loggedInUser = _jwtTokenRepository.ExtractUserDataFromToken(requestToken);

            if (request.CurrentChunk==9)
            {
                Console.WriteLine("dd");
            }

            var loggedInUser = new UserDto() { id = Guid.Parse("b39c91c0-c518-42da-8d56-c18b51dd7394") };

            var blobTableClient = _blobClientFactory.BlobTableClient(TableName.StashChunkDetail);

            var stashChunkDetail = blobTableClient
                .Query<StashChunkDetail>(x => x.RowKey == request.UploadToken.ToString()
                                              //&& x.UserId == loggedInUser.id
                                              )
                .SingleOrDefault();

          
            if (stashChunkDetail != null)
            {
                var currentChunkSize = request.Data.SizeMB();
                var isSizeOutOfDeal = stashChunkDetail.TotalUploadedSizeMB + currentChunkSize > stashChunkDetail.FileSizeMB;
                var containerName = Enum.Parse<ContainerName>(stashChunkDetail.PartitionKey);

                if (isSizeOutOfDeal)
                {
                    //delete table 
                    // ReSharper disable once MethodSupportsCancellation
                    await blobTableClient.DeleteEntityAsync(stashChunkDetail.PartitionKey, stashChunkDetail.RowKey);

                    //delete commit data form blob storage
                    await _fileUploadService.DeleteObjectAsync(containerName, stashChunkDetail.RowKey);

                    return BadRequest($"the size of chunks is more than {stashChunkDetail.FileSizeMB} MB");
                }

                if (request.CurrentChunk > stashChunkDetail.TotalChunks || request.CurrentChunk < 0)
                {
                    //delete table 
                    // ReSharper disable once MethodSupportsCancellation
                    //await blobTableClient.DeleteEntityAsync(stashChunkDetail.PartitionKey, stashChunkDetail.RowKey);

                    //delete commit data form blob storage
                    //await _fileUploadService.DeleteObjectAsync(containerName, stashChunkDetail.RowKey);

                    return BadRequest($"total chunks should be between 0 and {stashChunkDetail.TotalChunks}");
                }


                var fileChunkDto = new FileChunkDto()
                {
                    FileFormat = stashChunkDetail.FileFormat,
                    ContainerName = containerName,
                    FileName = stashChunkDetail.RowKey,
                    LastChunk = request.LastChunk,
                    Data = request.Data,
                    AccessTier = stashChunkDetail.AccessTier,
                    CurrentChunk = request.CurrentChunk,
                    TotalChunks = stashChunkDetail.TotalChunks
                };

                await _fileUploadService.UploadChunkAsync(fileChunkDto, cancellationToken);

                if (request.LastChunk)
                {
                    var objectIndex = new ObjectFile()
                    {
                        PartitionKey = fileChunkDto.ContainerName.ToString(),
                        RowKey = fileChunkDto.FileName,
                        FileFormat = fileChunkDto.FileFormat,
                        UserId = loggedInUser.id,
                        HaveUse = false,
                        Timestamp = DateTimeOffset.UtcNow,
                        Token = TokenGenerator.GenerateNewRngCrypto(),
                        ETag = ETag.All
                    };

                    // ReSharper disable once MethodSupportsCancellation
                    await _blobClientFactory.BlobTableClient(TableName.IndexObjectFile)
                        .AddEntityAsync(objectIndex);

                    // ReSharper disable once MethodSupportsCancellation
                    await _blobClientFactory.BlobTableClient(TableName.StashChunkDetail)
                        .DeleteEntityAsync(fileChunkDto.ContainerName.ToString(), fileChunkDto.FileName);

                    return Ok(new TokenResponse() { Token = objectIndex.Token });
                }

                stashChunkDetail.TotalUploadedChunks += 1;
                stashChunkDetail.TotalUploadedSizeMB += request.Data.SizeMB();

                // ReSharper disable once MethodSupportsCancellation
                await blobTableClient.UpdateEntityAsync(stashChunkDetail, ETag.All);

                return StatusCode(StatusCodes.Status201Created, new ChunkUploadResponse()
                {
                    TotalUploadedChunks = stashChunkDetail.TotalUploadedChunks,
                    TotalUploadedSizeMB = stashChunkDetail.TotalUploadedSizeMB
                });
            }

            return BadRequest("Please Request New Upload Token");
        }



        [HttpPost("Profile")]
        //[AuthorizeByIdentityServer]
        public async Task<IActionResult> RequestUploadProfileImageToken([FromBody] ImageUploadRequest request)
        {
            //var requestToken = _jwtTokenRepository.GetJwtToken();
            //var loggedInUser = _jwtTokenRepository.ExtractUserDataFromToken(requestToken);
            var loggedInUser = new UserDto(){id = Guid.Parse("b39c91c0-c518-42da-8d56-c18b51dd7394") };

            var stashChunkDetail = new StashChunkDetail()
            {
                FileFormat = request.FileFormat,
                PartitionKey = ContainerName.profile.ToString(),
                RowKey = Guid.NewGuid().ToString(),
                UserId = loggedInUser.id,
                AccessTier = AccessTier.Archive.ToString(),
                ETag = ETag.All,
                FileSizeMB = request.FileSizeMB+0.01,
                Timestamp = DateTimeOffset.UtcNow,
                TotalChunks = request.TotalChunks,
                TotalUploadedChunks = 0,
                TotalUploadedSizeMB = 0
            };
            AccessTier accessTier = (AccessTier)stashChunkDetail.AccessTier;
            var blobTableClient = _blobClientFactory.BlobTableClient(TableName.StashChunkDetail);

            await blobTableClient.AddEntityAsync(stashChunkDetail);

            return Ok(new TokenResponse() { Token = stashChunkDetail.RowKey });
        }



        [HttpPost("Image")]
        [AuthorizeByIdentityServer]
        public async Task<IActionResult> RequestUploadImageToken([FromBody] ImageUploadRequest request)
        {
            var requestToken = _jwtTokenRepository.GetJwtToken();
            var loggedInUser = _jwtTokenRepository.ExtractUserDataFromToken(requestToken);

            var stashChunkDetail = new StashChunkDetail()
            {
                FileFormat = request.FileFormat,
                PartitionKey = ContainerName.image.ToString(),
                RowKey = Guid.NewGuid().ToString(),
                UserId = loggedInUser.id,
                AccessTier = AccessTier.Archive.ToString(),
                ETag = ETag.All,
                FileSizeMB = request.FileSizeMB + 0.01,
                Timestamp = DateTimeOffset.UtcNow,
                TotalChunks = request.TotalChunks,
                TotalUploadedChunks = 0,
                TotalUploadedSizeMB = 0
            };
            var blobTableClient = _blobClientFactory.BlobTableClient(TableName.StashChunkDetail);

            await blobTableClient.AddEntityAsync(stashChunkDetail);

            return Ok(new TokenResponse() { Token = stashChunkDetail.RowKey });
        }


        [HttpPost("Audio")]
        [AuthorizeByIdentityServer]
        public async Task<IActionResult> RequestUploadProfileImageToken([FromBody] AudioUploadRequest request)
        {
            var requestToken = _jwtTokenRepository.GetJwtToken();
            var loggedInUser = _jwtTokenRepository.ExtractUserDataFromToken(requestToken);

            var stashChunkDetail = new StashChunkDetail()
            {
                FileFormat = request.FileFormat,
                PartitionKey = ContainerName.audio.ToString(),
                RowKey = Guid.NewGuid().ToString(),
                UserId = loggedInUser.id,
                AccessTier = AccessTier.Archive.ToString(),
                ETag = ETag.All,
                FileSizeMB = request.FileSizeMB + 0.01,
                Timestamp = DateTimeOffset.UtcNow,
                TotalChunks = request.TotalChunks,
                TotalUploadedChunks = 0,
                TotalUploadedSizeMB = 0
            };
            var blobTableClient = _blobClientFactory.BlobTableClient(TableName.StashChunkDetail);

            await blobTableClient.AddEntityAsync(stashChunkDetail);

            return Ok(new TokenResponse() { Token = stashChunkDetail.RowKey });
        }


        [HttpPost("Video")]
        [AuthorizeByIdentityServer]
        public async Task<IActionResult> RequestUploadVideoToken([FromBody] VideoUploadRequest request)
        {
            var requestToken = _jwtTokenRepository.GetJwtToken();
            var loggedInUser = _jwtTokenRepository.ExtractUserDataFromToken(requestToken);

            var stashChunkDetail = new StashChunkDetail()
            {
                FileFormat = request.FileFormat,
                PartitionKey = ContainerName.video.ToString(),
                RowKey = Guid.NewGuid().ToString(),
                UserId = loggedInUser.id,
                AccessTier = AccessTier.Archive.ToString(),
                ETag = ETag.All,
                FileSizeMB = request.FileSizeMB + 0.01,
                Timestamp = DateTimeOffset.UtcNow,
                TotalChunks = request.TotalChunks,
                TotalUploadedChunks = 0,
                TotalUploadedSizeMB = 0
            };
            var blobTableClient = _blobClientFactory.BlobTableClient(TableName.StashChunkDetail);

            await blobTableClient.AddEntityAsync(stashChunkDetail);

            return Ok(new TokenResponse() { Token = stashChunkDetail.RowKey });
        }


        [HttpPost("Other")]
        [AuthorizeByIdentityServer]
        public async Task<IActionResult> RequestUploadOtherToken([FromBody] OtherUploadRequest request)
        {
            var requestToken = _jwtTokenRepository.GetJwtToken();
            var loggedInUser = _jwtTokenRepository.ExtractUserDataFromToken(requestToken);

            var stashChunkDetail = new StashChunkDetail()
            {
                FileFormat = request.FileFormat,
                PartitionKey = ContainerName.other.ToString(),
                RowKey = Guid.NewGuid().ToString(),
                UserId = loggedInUser.id,
                AccessTier = AccessTier.Archive.ToString(),
                ETag = ETag.All,
                FileSizeMB = request.FileSizeMB + 0.01,
                Timestamp = DateTimeOffset.UtcNow,
                TotalChunks = request.TotalChunks,
                TotalUploadedChunks = 0,
                TotalUploadedSizeMB = 0
            };
            var blobTableClient = _blobClientFactory.BlobTableClient(TableName.StashChunkDetail);

            await blobTableClient.AddEntityAsync(stashChunkDetail);

            return Ok(new TokenResponse() { Token = stashChunkDetail.RowKey });
        }
    }
}
