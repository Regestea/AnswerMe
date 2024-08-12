using Microsoft.AspNetCore.Mvc;
using ObjectStorage.Api.Context;
using ObjectStorage.Api.Entities;
using Azure.Storage.Blobs.Models;
using System.Net;
using IdentityServer.Shared.Client.Attributes;
using IdentityServer.Shared.Client.Repositories.Interfaces;
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
using Models.Shared.Requests.Shared;
using Models.Shared.Requests.Upload;
using Models.Shared.Responses.ObjectStorage;
using ObjectStorage.Api.DTOs;
using ObjectStorage.Api.Extensions;

namespace ObjectStorage.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AuthorizeByIdentityServer]
    public class ObjectStorageController : ControllerBase
    {
        private readonly IFileUploadService _fileUploadService;
        private readonly IBlobClientFactory _blobClientFactory;
        private readonly IJwtTokenRepository _jwtTokenRepository;
        private readonly IBlurHashService _blurHashService;


        public ObjectStorageController(IFileUploadService fileUploadService, IBlobClientFactory blobClientFactory,
            IJwtTokenRepository jwtTokenRepository, IBlurHashService blurHashService)
        {
            _fileUploadService = fileUploadService;
            _blobClientFactory = blobClientFactory;
            _jwtTokenRepository = jwtTokenRepository;
            _blurHashService = blurHashService;
        }


        /// <summary>
        /// Upload a file chunk.
        /// </summary>
        /// <param name="request">File chunk request details.</param>
        /// <param name="cancellationToken">Cancellation token for asynchronous operation.</param>
        /// <returns>
        /// Returns an OK status along with a <see cref="TokenResponse"/> if the upload is successful.
        /// Returns Ok with TokenResponse
        /// ReturnsCreated with Upload Status
        /// Returns a BadRequest with an error message if there's an issue with the upload.
        /// </returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ChunkUploadResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ChunkUploadResponse))]
        public async Task<IActionResult> UploadChunkAsync([FromBody] FileChunkRequest request,
            CancellationToken cancellationToken = default)
        {
            var requestToken = _jwtTokenRepository.GetJwtToken();
            var loggedInUser = _jwtTokenRepository.ExtractUserDataFromToken(requestToken);

            var blobTableClient = _blobClientFactory.BlobTableClient(TableName.StashChunkDetail);

            var stashChunkDetail = blobTableClient
                .Query<StashChunkDetail>(x => x.RowKey == request.UploadToken && x.UserId == loggedInUser.id)
                .SingleOrDefault();

            if (stashChunkDetail != null)
            {
                var currentChunkSize = request.Data.SizeMB();
                var isSizeOutOfDeal = stashChunkDetail.TotalUploadedSizeMb + currentChunkSize >
                                      stashChunkDetail.FileSizeMb;
                var containerName = Enum.Parse<ContainerName>(stashChunkDetail.PartitionKey);

                if (isSizeOutOfDeal)
                {
                    //delete table 
                    // ReSharper disable once MethodSupportsCancellation
                    await blobTableClient.DeleteEntityAsync(stashChunkDetail.PartitionKey, stashChunkDetail.RowKey);

                    //delete commit data form blob storage
                    await _fileUploadService.DeleteObjectAsync(containerName, stashChunkDetail.RowKey);

                    return BadRequest(
                        $"the size of chunks is more than {stashChunkDetail.FileSizeMb} MB please request new upload token");
                }

                var isLast = (request.Data.SizeMB() + stashChunkDetail.TotalUploadedSizeMb) >=
                             stashChunkDetail.FileSizeMb;

                int totalUploadedChunks = stashChunkDetail.TotalUploadedChunks;
                int currentChunk;

                if (totalUploadedChunks == 0)
                {
                    currentChunk = 0;
                }
                else
                {
                    currentChunk = totalUploadedChunks + 1;
                }


                var fileChunkDto = new FileChunkDto()
                {
                    FileFormat = stashChunkDetail.FileFormat,
                    ContainerName = containerName,
                    FileName = stashChunkDetail.RowKey,
                    Data = request.Data,
                    AccessTier = stashChunkDetail.AccessTier,
                    CurrentChunk = currentChunk,
                    TotalUploadedChunks = stashChunkDetail.TotalUploadedChunks
                };

                await _fileUploadService.UploadChunkAsync(fileChunkDto, cancellationToken);


                stashChunkDetail.TotalUploadedChunks += 1;
                stashChunkDetail.TotalUploadedSizeMb += request.Data.SizeMB();

                // ReSharper disable once MethodSupportsCancellation
                await blobTableClient.UpdateEntityAsync(stashChunkDetail, ETag.All);

                var responseChunkProgress = new ChunkUploadResponse()
                {
                    TotalUploadedChunks = stashChunkDetail.TotalUploadedChunks,
                    TotalUploadedSizeMB = stashChunkDetail.TotalUploadedSizeMb
                };

                return StatusCode(StatusCodes.Status201Created, responseChunkProgress);
            }

            return BadRequest("Please Request New Upload Token");
        }


        /// <summary>
        /// Finalize the upload process for a file using the specified upload token.
        /// </summary>
        /// <param name="request">The unique token associated with the upload and blurhash</param>
        /// <returns>
        /// Returns an OK status along with a <see cref="TokenResponse"/> containing the finalized upload token.
        /// </returns>
        /// <response code="200">Successfully finalized the file upload.</response>
        /// <response code="400">Invalid request, such as attempting to finalize without uploading chunks.</response>
        /// <response code="404">The specified upload token or user was not found.</response>
        [HttpPost("Finalize")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TokenResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> FinalizeUpload([FromBody] TokenRequest request)
        {
            var requestToken = _jwtTokenRepository.GetJwtToken();
            var loggedInUser = _jwtTokenRepository.ExtractUserDataFromToken(requestToken);

            var blobTableClient = _blobClientFactory.BlobTableClient(TableName.StashChunkDetail);

            var stashChunkDetail = blobTableClient
                .Query<StashChunkDetail>(x => x.RowKey == request.Token && x.UserId == loggedInUser.id)
                .SingleOrDefault();
            if (stashChunkDetail == null)
            {
                return NotFound();
            }

            if (stashChunkDetail.TotalUploadedChunks < 1 || stashChunkDetail.TotalUploadedSizeMb == 0)
            {
                ModelState.AddModelError(nameof(request.Token), "Please upload more chunk");
                return BadRequest(ModelState);
            }

            var finalizeResult = await _fileUploadService.FinalizeUpload(new FinalizeUploadDto()
            {
                AccessTier = stashChunkDetail.AccessTier,
                ContainerName = stashChunkDetail.PartitionKey,
                FileFormat = stashChunkDetail.FileFormat,
                FileName = stashChunkDetail.RowKey
            });

            if (finalizeResult.IsValidationFailure)
            {
                ModelState.AddModelError("file",finalizeResult.AsValidationFailure.Error);
                return BadRequest(ModelState);
            }

            var objectIndex = new ObjectFile()
            {
                PartitionKey = stashChunkDetail.PartitionKey,
                RowKey = stashChunkDetail.RowKey,
                FileFormat = stashChunkDetail.FileFormat,
                UserId = loggedInUser.id,
                HaveUse = false,
                BlurHash = stashChunkDetail.BlurHash,
                Timestamp = DateTimeOffset.UtcNow,
                Token = TokenGenerator.GenerateNewToken(),
                ETag = ETag.All
            };
            

            await _blobClientFactory.BlobTableClient(TableName.IndexObjectFile).AddEntityAsync(objectIndex);


            await _blobClientFactory.BlobTableClient(TableName.StashChunkDetail)
                .DeleteEntityAsync(stashChunkDetail.PartitionKey, stashChunkDetail.RowKey);

            var responseToken = new TokenResponse() { FieldName = "Upload Token", Token = objectIndex.Token };

            return Ok(responseToken);
        }



        /// <summary>
        /// Request an upload token for a file of other type.
        /// </summary>
        /// <param name="request">Other file upload request details.</param>
        /// <returns>
        /// Returns an OK status along with a <see cref="TokenResponse"/> containing the upload token.
        /// </returns>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TokenResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("RequestUploadToken")]
        public async Task<IActionResult> RequestUploadTokenAsync([FromBody] UploadRequest request)
        {
            var requestToken = _jwtTokenRepository.GetJwtToken();
            var loggedInUser = _jwtTokenRepository.ExtractUserDataFromToken(requestToken);
            
            var stashChunkDetail = new StashChunkDetail()
            {
                FileFormat = request.FileFormat,
                PartitionKey = request.FileType.ToString(),
                RowKey = Guid.NewGuid().ToString(),
                UserId = loggedInUser.id,
                AccessTier = AccessTier.Archive.ToString(),
                ETag = ETag.All,
                FileSizeMb = request.FileSizeMb + 0.01,
                Timestamp = DateTimeOffset.UtcNow,
                TotalUploadedChunks = 0,
                TotalUploadedSizeMb = 0
            };
            
      
            if (request.FileType is FileType.image or FileType.video or FileType.profile && !string.IsNullOrWhiteSpace(request.BlurHash))
            {
                var isValidBlurHash = await _blurHashService.ValidateBlurHash(request.BlurHash);

                if (isValidBlurHash.AsT0.Value)
                {
                    stashChunkDetail.BlurHash = request.BlurHash;
                }
            }
            
            
            var blobTableClient = _blobClientFactory.BlobTableClient(TableName.StashChunkDetail);

            await blobTableClient.AddEntityAsync(stashChunkDetail);

            return Ok(new TokenResponse() { FieldName = "Upload Token", Token = stashChunkDetail.RowKey });
        }
    }
}