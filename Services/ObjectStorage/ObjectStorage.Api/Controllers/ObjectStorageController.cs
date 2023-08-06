using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ObjectStorage.Api.Context;
using ObjectStorage.Api.Entities;
using System.Reflection.Metadata;
using System.Text;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using static System.Reflection.Metadata.BlobBuilder;
using System.Drawing;
using System.IO;
using System.Net;
using ObjectStorage.Api.Services.InterFaces;


namespace ObjectStorage.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ObjectStorageController : ControllerBase
    {
        private readonly IFileUploadService _fileUploadService;

        public ObjectStorageController(IFileUploadService fileUploadService)
        {
            _fileUploadService = fileUploadService;
        }

        [HttpPost]
        // add validation for file Name and stream to upload only valid format not exe or .....
        // add cancel token for this upload async
        public async Task<IActionResult> Upload(Stream stream, string fileName, CancellationToken cancellationToken = default)
        {
            // Upload the object asynchronously with a cancellation token
            try
            {
                await _fileUploadService.UploadObjectAsync(ContainerName.image, fileName, stream, AccessTier.Cold, cancellationToken);
            }
            catch (TaskCanceledException)
            {
                // Handle cancellation of the upload operation
                return StatusCode((int)HttpStatusCode.RequestTimeout, "The upload operation was cancelled.");
            }

            // Return a successful result
            return Ok();
        }
    }
}
