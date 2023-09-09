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
        private ObjectStorageService.ObjectStorageServiceClient _storageServiceClient;

        public FileStorageService(ObjectStorageService.ObjectStorageServiceClient storageServiceClient)
        {
            _storageServiceClient = storageServiceClient;
        }

        public async Task<GetObjectPathResponse> GetObjectPathAsync(Guid userId, string token)
        {
            var request = new GetObjectPathRequest()
            {
                UserId = userId.ToString(),
                Token =token
            };

            return await _storageServiceClient.GetObjectPathByTokenAsync(request);
        }


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
