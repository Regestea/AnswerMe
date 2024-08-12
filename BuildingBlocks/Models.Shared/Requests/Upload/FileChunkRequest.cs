using Models.Shared.Attribute;

namespace Models.Shared.Requests.Upload
{
    public class FileChunkRequest
    {
        public string UploadToken { get; set; } = null!;
        [FileSizeMegabyte(0.000001,4)]
        public required byte[] Data { get; set; }
    }
}
