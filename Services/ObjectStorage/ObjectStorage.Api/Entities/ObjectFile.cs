using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Net.Sockets;
using Azure;
using Azure.Data.Tables;

namespace ObjectStorage.Api.Entities
{
    public class ObjectFile : ITableEntity
    {
        //blobName (Guid file name)
        public string RowKey { get; set; }

        //containerName it can be a number like 879435789098 and should be uniq
        public string PartitionKey { get; set; }

        public DateTimeOffset? Timestamp { get; set; }

        public ETag ETag { get; set; }

        [Required]
        public string FileFormat { get; set; } = null!;

        public string? BlurHash { get; set; }

        [Required]
        public bool HaveUse { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public string Token { get; set; } = null!;

        [NotMapped]
        public string FullPath => $"{PartitionKey}/{RowKey}.{FileFormat}";

    }
}
