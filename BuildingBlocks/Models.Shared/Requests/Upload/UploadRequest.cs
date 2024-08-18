using System.ComponentModel.DataAnnotations;
using Models.Shared.Attribute;

namespace Models.Shared.Requests.Upload;

public class UploadRequest
{
    public FileType FileType { get; set; }
    
    [MaxLength(28, ErrorMessage = "the component size should be 4 x 3 and string length 28")]
    [MinLength(28, ErrorMessage = "the component size should be 4 x 3 and string length 28")]
    public string? BlurHash { get; set; }

    [MaxLength(50)] 
    [Required] 
    public string FileName { get; set; } = null!;

    [RangeAllowed(nameof(FileType))]
    public double FileSizeMb { get; set; }

    [FileAllowed(nameof(FileType))] 
    public string FileFormat { get; set; } = null!;
}