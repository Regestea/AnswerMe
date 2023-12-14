using Azure.Storage.Blobs.Models;
using ObjectStorage.Api.Entities;

namespace ObjectStorage.Api.DTOs
{
    public class FileChunkDto
    {
        public required string AccessTier { get; set; }
        public required string FileFormat { get; set; }
        public required string FileName { get; set; }
        public int TotalUploadedChunks { get; set; }
        public ContainerName ContainerName { get; set; }
        public required byte[] Data { get; set; }
        public int CurrentChunk { get; set; }
        public bool LastChunk { get; set; }
    }
}
