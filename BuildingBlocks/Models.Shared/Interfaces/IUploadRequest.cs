namespace Models.Shared.Interfaces;

public interface IUploadRequest
{
    double FileSizeMB { get; set; }
    string FileFormat { get; set; }
}