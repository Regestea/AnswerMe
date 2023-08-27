using Azure.Storage.Blobs.Models;
using ObjectStorage.Api.Entities;

namespace ObjectStorage.Api.DTOs
{
    public class FileChunkDto
    {
        public AccessTier AccessTier { get; set; }
        public string FileFormat { get; set; }
        public string FileName { get; set; }
        public int TotalChunks { get; set; }
        public ContainerName ContainerName { get; set; }
        public byte[] Data { get; set; }
        public int CurrentChunk { get; set; }
        public bool LastChunk { get; set; }
    }
}
