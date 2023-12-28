using Models.Shared.Responses.Shared;

namespace Models.Shared.Responses.ObjectStorage;

public class ChunkUploadMultiResponse
{
    public TokenResponse? TokenResponse { get; set; }

    public ChunkUploadResponse? ChunkUploadResponse { get; set; }
}