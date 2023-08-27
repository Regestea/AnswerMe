using Azure;
using Grpc.Core;
using Models.Shared.Requests.ObjectStorage;
using Models.Shared.Requests.Shared;
using ObjectStorage.Api.Context;
using ObjectStorage.Api.Entities;
using ObjectStorage.Api.Protos;
using ObjectStorage.Api.Services.InterFaces;

namespace ObjectStorage.Api.GrpcServices
{
    public class ObjectStorageGrpcService : ObjectStorageService.ObjectStorageServiceBase
    {
        private readonly IBlobClientFactory _blobClientFactory;

        public ObjectStorageGrpcService(IBlobClientFactory blobClientFactory)
        {
            _blobClientFactory = blobClientFactory;
        }


        public override async Task<GetObjectPathResponse> GetObjectPathByToken(GetObjectPathRequest request, ServerCallContext context)
        {
            Guid userId = Guid.Parse(request.UserId);

            var tableClient = _blobClientFactory.BlobTableClient(TableName.IndexObjectFile);

            var objectFile = tableClient
                .Query<ObjectFile>(x => x.HaveUse == false && x.UserId == userId && x.Token == request.Token)
                .SingleOrDefault();

            if (objectFile != null)
            {
                objectFile.HaveUse = true;
                objectFile.Token = "";
                await tableClient.UpdateEntityAsync(objectFile, ETag.All);

                return new GetObjectPathResponse() { FilePath = objectFile.FullPath, FileType = objectFile.PartitionKey };
            }

            return new GetObjectPathResponse() { FilePath = "", FileType = "" };
        }

        public override async Task<DeleteObjectResponse> DeleteObjectByPath(DeleteObjectRequest request, ServerCallContext context)
        {
            Guid userId = Guid.Parse(request.UserId);
            var path = request.FilePath.Split("/");

            var tableClient = _blobClientFactory.BlobTableClient(TableName.IndexObjectFile);
            var response = tableClient.Query<ObjectFile>(x => x.PartitionKey == path[0] && x.RowKey == path[1] && x.UserId == userId).SingleOrDefault();
            if (response != null)
            {
                response.HaveUse = false;
                await tableClient.UpdateEntityAsync(response, ETag.All);
            }

            return new DeleteObjectResponse();
        }
    }
}
