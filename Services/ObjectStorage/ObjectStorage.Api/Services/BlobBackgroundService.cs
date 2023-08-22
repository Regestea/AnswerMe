using ObjectStorage.Api.Context;
using ObjectStorage.Api.Entities;
using ObjectStorage.Api.Services.InterFaces;

namespace ObjectStorage.Api.Services
{
    public class BlobBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        private bool _firstTime = true;

        public BlobBackgroundService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (_firstTime)
            {
                await SeedData();
                _firstTime = false;
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                await CleanGarbageData(stoppingToken);

                // Wait 24 hours before running the function again
                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
        }

        private async Task SeedData()
        {
            var scope = _serviceProvider.CreateScope();
            var _BlobClient = scope.ServiceProvider.GetRequiredService<IBlobClientFactory>();
            await _BlobClient.BlobTableClient().CreateIfNotExistsAsync();
            await _BlobClient.BlobStorageClient(ContainerName.image).CreateIfNotExistsAsync();
            await _BlobClient.BlobStorageClient(ContainerName.audio).CreateIfNotExistsAsync();
            await _BlobClient.BlobStorageClient(ContainerName.video).CreateIfNotExistsAsync();
            await _BlobClient.BlobStorageClient(ContainerName.profile).CreateIfNotExistsAsync();
            await _BlobClient.BlobStorageClient(ContainerName.other).CreateIfNotExistsAsync();
        }

        private async Task CleanGarbageData(CancellationToken stoppingToken)
        {

            var scope = _serviceProvider.CreateScope();
            var _BlobIndexClient = scope.ServiceProvider.GetRequiredService<IBlobClientFactory>().BlobTableClient();
            var _fileUploadService = scope.ServiceProvider.GetRequiredService<IFileUploadService>();
            var now = DateTimeOffset.UtcNow;

            var oneDayAgo = now.AddMinutes(-1);

            var filesToDelete = _BlobIndexClient
                .Query<ObjectFile>(x => x.Timestamp <= oneDayAgo && x.HaveUse == false).ToList();

            if (filesToDelete.Any())
            {
                foreach (ObjectFile file in filesToDelete)
                {
                    await _fileUploadService.DeleteObjectAsync(Enum.Parse<ContainerName>(file.PartitionKey),
                        file.RowKey + file.FileFormat);
                }

                foreach (ObjectFile file in filesToDelete)
                {
                    await _BlobIndexClient.DeleteEntityAsync(file.PartitionKey, file.RowKey, cancellationToken: stoppingToken);
                }
                filesToDelete.Clear();
            }
        }
    }
}
