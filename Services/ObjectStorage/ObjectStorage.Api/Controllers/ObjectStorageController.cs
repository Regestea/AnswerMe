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
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;


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

        //[HttpPost("UploadStream")]
        //public async Task<IActionResult> UploadStream()
        //{
        //    var body = HttpContext.Request.Body;
        //    var type = Request.ContentType;
        //    var uploadResponse =  _fileUploadService.UploadObjectAsync(ContainerName.profile, "my.png", HttpContext.Request.Body, AccessTier.Premium);
        //    return Ok();
        //}

        [HttpPost("UploadStream")]
        public async Task<IActionResult> UploadLargeFile([FromForm]string? fileName)
        {
            var request = HttpContext.Request;

            if (!request.HasFormContentType ||
                !MediaTypeHeaderValue.TryParse(request.ContentType, out var mediaTypeHeader) ||
                string.IsNullOrEmpty(mediaTypeHeader.Boundary.Value))
            {
                return new UnsupportedMediaTypeResult();
            }

            var reader = new MultipartReader(mediaTypeHeader.Boundary.Value, request.Body);
            var section = await reader.ReadNextSectionAsync();

            while (section != null)
            {
                var hasContentDispositionHeader = ContentDispositionHeaderValue.TryParse(section.ContentDisposition,
                    out var contentDisposition);

                if (hasContentDispositionHeader && contentDisposition.DispositionType.Equals("form-data") &&
                    !string.IsNullOrEmpty(contentDisposition.FileName.Value))
                {

                    // Upload the stream to the blob
                    using (var memoryStream = new MemoryStream())
                    {
                        await section.Body.CopyToAsync(memoryStream);
                        memoryStream.Seek(0, SeekOrigin.Begin);
                        //await _blobClientFactory.BlobStorageClient(ContainerName.image).UploadBlobAsync("test.png",memoryStream);
                        await _fileUploadService.UploadObjectAsync(ContainerName.profile, "some.png", memoryStream,
                            AccessTier.Hot);
                    }

                    return Ok();
                }

                section = await reader.ReadNextSectionAsync();
            }

            return BadRequest("No files data in the request.");
        }

        [HttpPost("Profile")]
        [AuthorizeByIdentityServer]
        public async Task<IActionResult> UploadProfileImage([FromForm] ImageUploadRequest request, CancellationToken cancellationToken = default)
        {
            var requestToken = _jwtTokenRepository.GetJwtToken();
            var loggedInUser = _jwtTokenRepository.ExtractUserDataFromToken(requestToken);

            try
            {
                var uploadResponse = await _fileUploadService.UploadObjectAsync(ContainerName.profile, request.FileName, request.Stream, AccessTier.Premium, cancellationToken);
                if (uploadResponse.IsT0)
                {
                    var objectIndex = new ObjectFile()
                    {
                        PartitionKey = ContainerName.profile.ToString(),
                        RowKey = uploadResponse.AsT0.Value.RowKey,
                        FileFormat = uploadResponse.AsT0.Value.FileFormat,
                        UserId = loggedInUser.id,
                        HaveUse = false,
                        Timestamp = DateTimeOffset.UtcNow,
                        Token = TokenGenerator.GenerateNewRngCrypto(),
                    };

                    await _blobClientFactory.BlobTableClient().AddEntityAsync(objectIndex, cancellationToken);

     
                    return Ok(new TokenResponse(){Token = objectIndex.Token});
                }

                throw new TaskCanceledException();
            }
            catch (TaskCanceledException)
            {
                // Handle cancellation of the upload operation
                return StatusCode((int)HttpStatusCode.RequestTimeout, "The upload operation was cancelled.");
            }
        }



        [HttpPost("Image")]
        [AuthorizeByIdentityServer]
        public async Task<IActionResult> UploadImage([FromForm] ImageUploadRequest request, CancellationToken cancellationToken = default)
        {
            var requestToken = _jwtTokenRepository.GetJwtToken();
            var loggedInUser = _jwtTokenRepository.ExtractUserDataFromToken(requestToken);

            try
            {
                var uploadResponse = await _fileUploadService.UploadObjectAsync(ContainerName.image, request.FileName, request.Stream, AccessTier.Hot, cancellationToken);
                if (uploadResponse.IsT0)
                {
                    var objectIndex = new ObjectFile()
                    {
                        PartitionKey = ContainerName.image.ToString(),
                        RowKey = uploadResponse.AsT0.Value.RowKey,
                        FileFormat = uploadResponse.AsT0.Value.FileFormat,
                        UserId = loggedInUser.id,
                        HaveUse = false,
                        Timestamp = DateTimeOffset.UtcNow,
                        Token = TokenGenerator.GenerateNewRngCrypto(),
                    };

                    await _blobClientFactory.BlobTableClient().AddEntityAsync(objectIndex, cancellationToken);


                    return Ok(new TokenResponse() { Token = objectIndex.Token });
                }

                throw new TaskCanceledException();
            }
            catch (TaskCanceledException)
            {
                // Handle cancellation of the upload operation
                return StatusCode((int)HttpStatusCode.RequestTimeout, "The upload operation was cancelled.");
            }
        }


        [HttpPost("Audio")]
        [AuthorizeByIdentityServer]
        public async Task<IActionResult> UploadProfileImage([FromForm] AudioUploadRequest request, CancellationToken cancellationToken = default)
        {
            var requestToken = _jwtTokenRepository.GetJwtToken();
            var loggedInUser = _jwtTokenRepository.ExtractUserDataFromToken(requestToken);

            try
            {
                var uploadResponse = await _fileUploadService.UploadObjectAsync(ContainerName.audio, request.FileName, request.Stream, AccessTier.Premium, cancellationToken);
                if (uploadResponse.IsT0)
                {
                    var objectIndex = new ObjectFile()
                    {
                        PartitionKey = ContainerName.audio.ToString(),
                        RowKey = uploadResponse.AsT0.Value.RowKey,
                        FileFormat = uploadResponse.AsT0.Value.FileFormat,
                        UserId = loggedInUser.id,
                        HaveUse = false,
                        Timestamp = DateTimeOffset.UtcNow,
                        Token = TokenGenerator.GenerateNewRngCrypto(),
                    };

                    await _blobClientFactory.BlobTableClient().AddEntityAsync(objectIndex, cancellationToken);


                    return Ok(new TokenResponse() { Token = objectIndex.Token });
                }

                throw new TaskCanceledException();
            }
            catch (TaskCanceledException)
            {
                // Handle cancellation of the upload operation
                return StatusCode((int)HttpStatusCode.RequestTimeout, "The upload operation was cancelled.");
            }
        }


        [HttpPost("Video")]
        [AuthorizeByIdentityServer]
        public async Task<IActionResult> UploadVideo([FromForm] VideoUploadRequest request, CancellationToken cancellationToken = default)
        {
            var requestToken = _jwtTokenRepository.GetJwtToken();
            var loggedInUser = _jwtTokenRepository.ExtractUserDataFromToken(requestToken);

            try
            {
                var uploadResponse = await _fileUploadService.UploadObjectAsync(ContainerName.video, request.FileName, request.Stream, AccessTier.Cold, cancellationToken);
                if (uploadResponse.IsT0)
                {
                    var objectIndex = new ObjectFile()
                    {
                        PartitionKey = ContainerName.video.ToString(),
                        RowKey = uploadResponse.AsT0.Value.RowKey,
                        FileFormat = uploadResponse.AsT0.Value.FileFormat,
                        UserId = loggedInUser.id,
                        HaveUse = false,
                        Timestamp = DateTimeOffset.UtcNow,
                        Token = TokenGenerator.GenerateNewRngCrypto(),
                    };

                    await _blobClientFactory.BlobTableClient().AddEntityAsync(objectIndex, cancellationToken);


                    return Ok(new TokenResponse() { Token = objectIndex.Token });
                }

                throw new TaskCanceledException();
            }
            catch (TaskCanceledException)
            {
                // Handle cancellation of the upload operation
                return StatusCode((int)HttpStatusCode.RequestTimeout, "The upload operation was cancelled.");
            }
        }


        [HttpPost("Other")]
        [AuthorizeByIdentityServer]
        public async Task<IActionResult> UploadOther([FromForm] OtherUploadRequest request, CancellationToken cancellationToken = default)
        {
            var requestToken = _jwtTokenRepository.GetJwtToken();
            var loggedInUser = _jwtTokenRepository.ExtractUserDataFromToken(requestToken);

            try
            {
                var uploadResponse = await _fileUploadService.UploadObjectAsync(ContainerName.other, request.FileName, request.Stream, AccessTier.Cold, cancellationToken);
                if (uploadResponse.IsT0)
                {
                    var objectIndex = new ObjectFile()
                    {
                        PartitionKey = ContainerName.other.ToString(),
                        RowKey = uploadResponse.AsT0.Value.RowKey,
                        FileFormat = uploadResponse.AsT0.Value.FileFormat,
                        UserId = loggedInUser.id,
                        HaveUse = false,
                        Timestamp = DateTimeOffset.UtcNow,
                        Token = TokenGenerator.GenerateNewRngCrypto(),
                    };

                    await _blobClientFactory.BlobTableClient().AddEntityAsync(objectIndex, cancellationToken);


                    return Ok(new TokenResponse() { Token = objectIndex.Token });
                }

                throw new TaskCanceledException();
            }
            catch (TaskCanceledException)
            {
                // Handle cancellation of the upload operation
                return StatusCode((int)HttpStatusCode.RequestTimeout, "The upload operation was cancelled.");
            }
        }
    }
}
