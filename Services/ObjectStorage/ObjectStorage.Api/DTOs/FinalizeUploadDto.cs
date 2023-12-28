using ObjectStorage.Api.Entities;

namespace ObjectStorage.Api.DTOs;

public class FinalizeUploadDto
{
    public required string AccessTier { get; set; }
    public required string ContainerName { get; set; }
    public required string FileFormat { get; set; }
    public required string FileName { get; set; }
}