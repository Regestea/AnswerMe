using Azure;
using Azure.Data.Tables;
using Azure.Storage.Blobs.Models;

namespace ObjectStorage.Api.Entities
{
    public class StashChunkDetail: ITableEntity
    {
        public double FileSizeMB { get; set; }
        public double TotalUploadedSizeMB { get; set; }
        public int TotalUploadedChunks { get; set; }
        public int TotalChunks { get; set; }
        public string FileFormat { get; set; } = null!;
        public Guid UserId { get; set; }
        public AccessTier AccessTier { get; set; }
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}
