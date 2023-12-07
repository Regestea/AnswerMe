using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ObjectStorage.Api.Protos;

namespace AnswerMe.Infrastructure.Services
{
    public class FileStorageService
    {
        private readonly ObjectStorageService.ObjectStorageServiceClient _storageServiceClient;

        public FileStorageService(ObjectStorageService.ObjectStorageServiceClient storageServiceClient)
        {
            _storageServiceClient = storageServiceClient;
        }


        /// <summary>
        /// Asynchronously gets the object path for a given user and authentication token.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="token">The authentication token.</param>
        /// <returns>The task object representing the asynchronous operation. The task result contains the response with the object path.</returns>
        public async Task<GetObjectPathResponse> GetObjectPathAsync(Guid userId, string token)
        {
                var request = new GetObjectPathRequest()
            {
                UserId = userId.ToString(),
                Token =token
            };

            return await _storageServiceClient.GetObjectPathByTokenAsync(request);
        }


        /// <summary>
        /// Deletes an object asynchronously.
        /// </summary>
        /// <param name="userId">The unique identifier of the user performing the deletion.</param>
        /// <param name="filePath">The path of the file to be deleted.</param>
        /// <returns>A Task that represents the asynchronous operation. The task result contains the response of the delete operation.</returns>
        public async Task<DeleteObjectResponse> DeleteObjectAsync(Guid userId , string filePath)
        {

            var request = new DeleteObjectRequest()
            {
                UserId = userId.ToString(),
                FilePath = filePath
            };

            return await _storageServiceClient.DeleteObjectByPathAsync(request);
        }
    }
}
