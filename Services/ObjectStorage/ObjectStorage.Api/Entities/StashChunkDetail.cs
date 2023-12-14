using Azure;
using Azure.Data.Tables;
using Azure.Storage.Blobs.Models;

namespace ObjectStorage.Api.Entities
{
    public class StashChunkDetail: ITableEntity
    {
        public double FileSizeMb { get; set; }
        public double TotalUploadedSizeMb { get; set; }
        public int TotalUploadedChunks { get; set; }
        public string FileFormat { get; set; } = null!;
        public string? BlurHash { get; set; }
        public Guid UserId { get; set; }
        public required string AccessTier { get; set; }
        public required string PartitionKey { get; set; }
        public required string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}
